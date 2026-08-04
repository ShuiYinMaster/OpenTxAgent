// TxTools.Agent / Core / DeepSeekClient.cs
// 直连 OpenAI 兼容 /v1/chat/completions 的薄客户端 (baseUrl 可配置)。
// 类名沿用历史命名,内部完全 provider 中立 —— 换用 DeepSeek / Kimi / Qwen / OpenAI / Ollama 
// 只需构造时传对应 baseUrl。网络要求: 目标 host 出站 HTTPS(Ollama 是本地 HTTP)可达。

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TxTools.Agent.Core
{
    public sealed class DeepSeekClient
    {
        public const string DefaultBaseUrl = "https://api.deepseek.com";

        private static readonly HttpClient Http = CreateHttp();
        private static readonly JsonSerializerSettings JsonSettings =
            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        private readonly string _apiKey;
        /// <summary>完整 endpoint,如 https://api.deepseek.com/v1/chat/completions</summary>
        private readonly string _endpoint;
        /// <summary>模型列表 endpoint,如 https://api.deepseek.com/v1/models</summary>
        private readonly string _modelsEndpoint;

        static DeepSeekClient()
        {
            // .NET Framework 默认可能不启用 TLS1.2,否则握手直接失败。
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
        }

        public DeepSeekClient(string apiKey) : this(apiKey, DefaultBaseUrl) { }

        public DeepSeekClient(string apiKey, string baseUrl)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("API key 不能为空。", nameof(apiKey));
            _apiKey = apiKey.Trim();
            var b = string.IsNullOrWhiteSpace(baseUrl) ? DefaultBaseUrl : baseUrl.Trim().TrimEnd('/');
            _endpoint = b + "/v1/chat/completions";
            _modelsEndpoint = b + "/v1/models";
        }

        /// <summary>
        /// 拉取当前 provider 真实的模型列表(OpenAI 兼容: GET /v1/models → {data: [{id: "..."}, ...]})。
        /// 五家 DeepSeek/Kimi/Qwen/OpenAI/Ollama 都支持。抛异常表示 API 不响应或返回格式不合规。
        /// </summary>
        public async Task<List<string>> ListModelsAsync(CancellationToken ct)
        {
            using (var msg = new HttpRequestMessage(HttpMethod.Get, _modelsEndpoint))
            {
                msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
                using (var resp = await Http.SendAsync(msg, HttpCompletionOption.ResponseContentRead, ct))
                {
                    var body = await resp.Content.ReadAsStringAsync();
                    if (!resp.IsSuccessStatusCode)
                        throw new LlmApiException((int)resp.StatusCode, ExtractError(body, resp.StatusCode), body);

                    var root = JObject.Parse(body);
                    var data = root["data"] as JArray;
                    if (data == null) return new List<string>();

                    var list = new List<string>();
                    foreach (var item in data)
                    {
                        var id = (string)item["id"];
                        if (!string.IsNullOrWhiteSpace(id)) list.Add(id);
                    }
                    return list;
                }
            }
        }

        public async Task<ChatResponse> SendAsync(ChatRequest request, CancellationToken ct)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            try
            {
                return await SendOnceAsync(request, ct);
            }
            catch (LlmApiException ex) when (ShouldRetryWithoutTemperature(ex, request))
            {
                System.Diagnostics.Debug.WriteLine(
                    "[DeepSeekClient] 400 temperature 不支持,自动去掉 temperature 重试: " + ex.Message);
                request.Temperature = null;
                return await SendOnceAsync(request, ct);
            }
        }

        private async Task<ChatResponse> SendOnceAsync(ChatRequest request, CancellationToken ct)
        {
            var json = JsonConvert.SerializeObject(request, JsonSettings);

            using (var msg = new HttpRequestMessage(HttpMethod.Post, _endpoint))
            {
                msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
                msg.Content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var resp = await Http.SendAsync(msg, HttpCompletionOption.ResponseContentRead, ct))
                {
                    var body = await resp.Content.ReadAsStringAsync();

                    if (!resp.IsSuccessStatusCode)
                        throw new LlmApiException((int)resp.StatusCode, ExtractError(body, resp.StatusCode), body);

                    var parsed = JsonConvert.DeserializeObject<ChatResponse>(body);
                    if (parsed == null || parsed.Choices == null || parsed.Choices.Count == 0)
                        throw new LlmApiException((int)resp.StatusCode, "API 响应为空或无 choices。", body);

                    SplitInlineThink(parsed);
                    return parsed;
                }
            }
        }

        /// <summary>流式发送：边收边回调文本分片，结束后返回拼装好的 assistant 消息(含 tool_calls)。
        /// 最后一个 SSE 包中的 usage 字段会写入 outUsage（如不为 null）。
        /// onReasoningDelta 用于推理模型的 reasoning_content 分片；普通模型不会触发。
        /// 返回的 ChatMessage 上,Content 与 ReasoningContent 均为聚合后的完整文本。</summary>
        public async Task<ChatMessage> SendStreamAsync(ChatRequest request, Action<string> onTextDelta,
            CancellationToken ct, Action<TokenUsage> onUsage = null, Action<string> onReasoningDelta = null)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            // 不显式要求的话,DeepSeek 流式不返回 token 用量。
            if (request.StreamOptions == null)
                request.StreamOptions = new StreamOptions { IncludeUsage = true };

            try
            {
                return await SendStreamOnceAsync(request, onTextDelta, ct, onUsage, onReasoningDelta);
            }
            catch (LlmApiException ex) when (ShouldRetryWithoutStreamOptions(ex, request))
            {
                System.Diagnostics.Debug.WriteLine(
                    "[DeepSeekClient] 400 stream_options 不支持,自动去掉后重试: " + ex.Message);
                request.StreamOptions = null;
                return await SendStreamOnceAsync(request, onTextDelta, ct, onUsage, onReasoningDelta);
            }
            catch (LlmApiException ex) when (ShouldRetryWithoutTemperature(ex, request))
            {
                System.Diagnostics.Debug.WriteLine(
                    "[DeepSeekClient] 400 temperature 不支持,自动去掉 temperature 重试: " + ex.Message);
                request.Temperature = null;
                return await SendStreamOnceAsync(request, onTextDelta, ct, onUsage, onReasoningDelta);
            }
        }

        private async Task<ChatMessage> SendStreamOnceAsync(ChatRequest request, Action<string> onTextDelta,
            CancellationToken ct, Action<TokenUsage> onUsage, Action<string> onReasoningDelta)
        {
            request.Stream = true;
            var json = JsonConvert.SerializeObject(request, JsonSettings);

            using (var msg = new HttpRequestMessage(HttpMethod.Post, _endpoint))
            {
                msg.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
                msg.Content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var resp = await Http.SendAsync(msg, HttpCompletionOption.ResponseHeadersRead, ct))
                {
                    if (!resp.IsSuccessStatusCode)
                    {
                        var body = await resp.Content.ReadAsStringAsync();
                        throw new LlmApiException((int)resp.StatusCode, ExtractError(body, resp.StatusCode), body);
                    }

                    var content = new StringBuilder();
                    var reasoning = new StringBuilder();
                    var toolAcc = new SortedDictionary<int, ToolCallAcc>();
                    TokenUsage lastUsage = null;

                    // 部分第三方代理端点(如阿里百炼代理的 deepseek 系列)不把思考放进
                    // reasoning_content 字段,而是把 <think>...</think> 原样塞在 content 里。
                    // 不处理的话标签会直接漏到聊天气泡上。这里做一个跨分片的状态机,
                    // 把 think 区间内的文本改走 reasoning 通道。
                    bool inThink = false;
                    var pending = new StringBuilder();   // 可能被标签截断的尾巴

                    using (var stream = await resp.Content.ReadAsStreamAsync())
                    using (var reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        string line;
                        while ((line = await reader.ReadLineAsync()) != null)
                        {
                            ct.ThrowIfCancellationRequested();
                            if (line.Length == 0 || !line.StartsWith("data:")) continue;

                            var data = line.Substring(5).Trim();
                            if (data == "[DONE]") break;

                            JObject chunk;
                            try { chunk = JObject.Parse(data); } catch { continue; }

                            // 顶层 usage（DeepSeek 在末尾 chunk 带回，需 stream_options.include_usage）
                            var usageTok = chunk["usage"];
                            if (usageTok != null && usageTok.Type == JTokenType.Object)
                            {
                                try { lastUsage = usageTok.ToObject<TokenUsage>(); } catch { }
                            }

                            var choices = chunk["choices"] as JArray;
                            if (choices == null || choices.Count == 0) continue;
                            var delta = choices[0]["delta"] as JObject;
                            if (delta == null) continue;

                            // 思考内容分片(推理模型)。DeepSeek 先吐完 reasoning_content 再吐 content。
                            var rtok = delta["reasoning_content"];
                            if (rtok != null && rtok.Type == JTokenType.String)
                            {
                                var rfrag = (string)rtok;
                                if (rfrag.Length > 0)
                                {
                                    reasoning.Append(rfrag);
                                    if (onReasoningDelta != null) onReasoningDelta(rfrag);
                                }
                            }

                            var ctok = delta["content"];
                            if (ctok != null && ctok.Type == JTokenType.String)
                            {
                                var frag = (string)ctok;
                                if (frag.Length > 0)
                                    RouteThinkAware(frag, pending, ref inThink,
                                        content, reasoning, onTextDelta, onReasoningDelta);
                            }

                            var tcs = delta["tool_calls"] as JArray;
                            if (tcs != null)
                                foreach (var tc in tcs)
                                {
                                    int idx = tc["index"] != null ? (int)tc["index"] : 0;
                                    ToolCallAcc acc;
                                    if (!toolAcc.TryGetValue(idx, out acc)) { acc = new ToolCallAcc(); toolAcc[idx] = acc; }
                                    if (tc["id"] != null && tc["id"].Type == JTokenType.String) acc.Id = (string)tc["id"];
                                    var fn = tc["function"] as JObject;
                                    if (fn != null)
                                    {
                                        if (fn["name"] != null && fn["name"].Type == JTokenType.String) acc.Name = (string)fn["name"];
                                        if (fn["arguments"] != null && fn["arguments"].Type == JTokenType.String) acc.Args.Append((string)fn["arguments"]);
                                    }
                                }
                        }
                    }

                    // 流结束:把还压在缓冲里的尾巴放出来(不可能再是标签的一半了)
                    if (pending.Length > 0)
                    {
                        var tail = pending.ToString();
                        pending.Length = 0;
                        if (inThink) { reasoning.Append(tail); if (onReasoningDelta != null) onReasoningDelta(tail); }
                        else { content.Append(tail); if (onTextDelta != null) onTextDelta(tail); }
                    }

                    // 回调 usage
                    if (lastUsage != null && onUsage != null) onUsage(lastUsage);

                    var message = new ChatMessage("assistant", content.Length > 0 ? content.ToString() : null);
                    if (reasoning.Length > 0) message.ReasoningContent = reasoning.ToString();
                    if (toolAcc.Count > 0)
                    {
                        message.ToolCalls = new List<ToolCall>();
                        foreach (var kv in toolAcc)
                            message.ToolCalls.Add(new ToolCall
                            {
                                Id = kv.Value.Id,
                                Type = "function",
                                Function = new FunctionCall { Name = kv.Value.Name, Arguments = kv.Value.Args.ToString() }
                            });
                    }
                    return message;
                }
            }
        }

        private const string ThinkOpen = "<think>";
        private const string ThinkClose = "</think>";

        /// <summary>
        /// 按 &lt;think&gt; 标签把分片分流到正文/思考两个通道。
        /// 标签可能跨 SSE 分片被切断,所以用 pending 缓冲一小段:
        /// 只要尾部可能是某个标签的前缀,就先压住不发,等下一片拼上再判断。
        /// </summary>
        private static void RouteThinkAware(
            string frag, StringBuilder pending, ref bool inThink,
            StringBuilder content, StringBuilder reasoning,
            Action<string> onTextDelta, Action<string> onReasoningDelta)
        {
            pending.Append(frag);

            while (true)
            {
                var buf = pending.ToString();
                var marker = inThink ? ThinkClose : ThinkOpen;

                int idx = buf.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
                if (idx >= 0)
                {
                    var before = buf.Substring(0, idx);
                    Emit(before, inThink, content, reasoning, onTextDelta, onReasoningDelta);

                    pending.Length = 0;
                    pending.Append(buf.Substring(idx + marker.Length));
                    inThink = !inThink;
                    continue;
                }

                // 没有完整标签:把"肯定安全"的部分放出去,尾部可能是标签前缀的留着
                int keep = SafeTailLength(buf, marker);
                if (keep > 0)
                {
                    Emit(buf.Substring(0, buf.Length - keep), inThink, content, reasoning,
                        onTextDelta, onReasoningDelta);
                    pending.Length = 0;
                    pending.Append(buf.Substring(buf.Length - keep));
                }
                else
                {
                    Emit(buf, inThink, content, reasoning, onTextDelta, onReasoningDelta);
                    pending.Length = 0;
                }
                return;
            }
        }

        private static void Emit(string text, bool inThink,
            StringBuilder content, StringBuilder reasoning,
            Action<string> onTextDelta, Action<string> onReasoningDelta)
        {
            if (string.IsNullOrEmpty(text)) return;
            if (inThink)
            {
                reasoning.Append(text);
                if (onReasoningDelta != null) onReasoningDelta(text);
            }
            else
            {
                content.Append(text);
                if (onTextDelta != null) onTextDelta(text);
            }
        }

        /// <summary>末尾有多少字符可能是 marker 被截断的前缀,需要压住等下一片。</summary>
        private static int SafeTailLength(string buf, string marker)
        {
            int max = Math.Min(marker.Length - 1, buf.Length);
            for (int n = max; n > 0; n--)
            {
                if (string.Compare(buf, buf.Length - n, marker, 0, n,
                        StringComparison.OrdinalIgnoreCase) == 0)
                    return n;
            }
            return 0;
        }

        private sealed class ToolCallAcc
        {
            public string Id;
            public string Name;
            public readonly StringBuilder Args = new StringBuilder();
        }

        /// <summary>
        /// 非流式响应里若 content 内嵌 &lt;think&gt;...&lt;/think&gt;,拆到 ReasoningContent。
        /// 同上:部分第三方代理端点不返回独立的 reasoning_content 字段。
        /// </summary>
        private static void SplitInlineThink(ChatResponse resp)
        {
            if (resp == null || resp.Choices == null) return;
            foreach (var ch in resp.Choices)
            {
                var msg = ch != null ? ch.Message : null;
                if (msg == null || string.IsNullOrEmpty(msg.Content)) continue;
                if (msg.Content.IndexOf(ThinkOpen, StringComparison.OrdinalIgnoreCase) < 0) continue;

                var think = new StringBuilder();
                var body = new StringBuilder();
                var text = msg.Content;
                int pos = 0;

                while (pos < text.Length)
                {
                    int open = text.IndexOf(ThinkOpen, pos, StringComparison.OrdinalIgnoreCase);
                    if (open < 0) { body.Append(text, pos, text.Length - pos); break; }

                    body.Append(text, pos, open - pos);
                    int inner = open + ThinkOpen.Length;
                    int close = text.IndexOf(ThinkClose, inner, StringComparison.OrdinalIgnoreCase);

                    if (close < 0) { think.Append(text, inner, text.Length - inner); break; }

                    think.Append(text, inner, close - inner);
                    pos = close + ThinkClose.Length;
                }

                msg.Content = body.ToString().Trim();
                if (think.Length > 0)
                {
                    msg.ReasoningContent = string.IsNullOrEmpty(msg.ReasoningContent)
                        ? think.ToString()
                        : msg.ReasoningContent + "\n" + think;
                }
            }
        }

        /// <summary>
        /// 判断是否应该"去掉 temperature 参数后重试":
        ///   - HTTP 400 (invalid_request_error)
        ///   - 错误消息含 "temperature"
        ///   - 当前 request 确实带了 temperature (未曾发生过重试)
        /// 已知触发场景:
        ///   - Kimi(Moonshot) k2 系列: "invalid temperature: only 1 is allowed for this model"
        ///   - OpenAI o1/o3 系列:      "'temperature' does not support 0.3 with this model. Only ..."
        /// 宽松匹配 —— 只要 400 + msg 含 temperature 就重试,不追具体表述,避免 provider 措辞变化。
        /// </summary>
        private static bool ShouldRetryWithoutTemperature(LlmApiException ex, ChatRequest request)
        {
            if (ex.StatusCode != 400) return false;
            if (request == null || !request.Temperature.HasValue) return false;
            var msg = ex.Message ?? "";
            return msg.IndexOf("temperature", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// 同上,针对 stream_options:部分 provider(自建/旧版 Ollama 等)不认这个字段会直接 400。
        /// 去掉后只是拿不到流式 token 用量,不影响正文与工具调用。
        /// </summary>
        private static bool ShouldRetryWithoutStreamOptions(LlmApiException ex, ChatRequest request)
        {
            if (ex.StatusCode != 400) return false;
            if (request == null || request.StreamOptions == null) return false;
            var msg = ex.Message ?? "";
            return msg.IndexOf("stream_options", StringComparison.OrdinalIgnoreCase) >= 0
                || msg.IndexOf("include_usage", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string ExtractError(string body, HttpStatusCode code)
        {
            try
            {
                var root = JObject.Parse(body);
                var err = root["error"];
                if (err != null)
                {
                    var message = (string)err["message"];
                    var type = (string)err["type"];
                    return $"[{(int)code} {type}] {message}";
                }
            }
            catch { /* 落到通用信息 */ }
            return $"[{(int)code}] {body}";
        }

        private static HttpClient CreateHttp()
        {
            var h = new HttpClient();
            h.Timeout = TimeSpan.FromMinutes(5);
            return h;
        }
    }

    public sealed class LlmApiException : Exception
    {
        public int StatusCode { get; }
        public string RawBody { get; }

        public LlmApiException(int statusCode, string message, string rawBody) : base(message)
        {
            StatusCode = statusCode;
            RawBody = rawBody;
        }
    }
}