// TxTools.Agent / Core / DeepSeekClient.cs
// 直连 https://api.deepseek.com/chat/completions 的薄客户端 (OpenAI 兼容)。
// 网络要求: PS 工作站需放行到 api.deepseek.com 的出站 HTTPS。

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
        // base_url 末尾的 v1 与模型版本无关；这里直接用根域 + /chat/completions。
        private const string Endpoint = "https://api.deepseek.com/chat/completions";

        private static readonly HttpClient Http = CreateHttp();
        private static readonly JsonSerializerSettings JsonSettings =
            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

        private readonly string _apiKey;

        static DeepSeekClient()
        {
            // .NET Framework 默认可能不启用 TLS1.2，否则握手直接失败。
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
        }

        public DeepSeekClient(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("API key 不能为空。", nameof(apiKey));
            _apiKey = apiKey.Trim();
        }

        public async Task<ChatResponse> SendAsync(ChatRequest request, CancellationToken ct)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var json = JsonConvert.SerializeObject(request, JsonSettings);

            using (var msg = new HttpRequestMessage(HttpMethod.Post, Endpoint))
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
                    return parsed;
                }
            }
        }

        /// <summary>流式发送：边收边回调文本分片，结束后返回拼装好的 assistant 消息(含 tool_calls)。
        /// 最后一个 SSE 包中的 usage 字段会写入 outUsage（如不为 null）。</summary>
        public async Task<ChatMessage> SendStreamAsync(ChatRequest request, Action<string> onTextDelta,
            CancellationToken ct, Action<TokenUsage> onUsage = null)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            request.Stream = true;
            var json = JsonConvert.SerializeObject(request, JsonSettings);

            using (var msg = new HttpRequestMessage(HttpMethod.Post, Endpoint))
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
                    var toolAcc = new SortedDictionary<int, ToolCallAcc>();
                    TokenUsage lastUsage = null;

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

                            // 顶层 usage（DeepSeek 在末尾 chunk 带回）
                            var usageTok = chunk["usage"];
                            if (usageTok != null && usageTok.Type == JTokenType.Object)
                            {
                                try { lastUsage = usageTok.ToObject<TokenUsage>(); } catch { }
                            }

                            var choices = chunk["choices"] as JArray;
                            if (choices == null || choices.Count == 0) continue;
                            var delta = choices[0]["delta"] as JObject;
                            if (delta == null) continue;

                            var ctok = delta["content"];
                            if (ctok != null && ctok.Type == JTokenType.String)
                            {
                                var frag = (string)ctok;
                                if (frag.Length > 0) { content.Append(frag); if (onTextDelta != null) onTextDelta(frag); }
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

                    // 回调 usage
                    if (lastUsage != null && onUsage != null) onUsage(lastUsage);

                    var message = new ChatMessage("assistant", content.Length > 0 ? content.ToString() : null);
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

        private sealed class ToolCallAcc
        {
            public string Id;
            public string Name;
            public readonly StringBuilder Args = new StringBuilder();
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
