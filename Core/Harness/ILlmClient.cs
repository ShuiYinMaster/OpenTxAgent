using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TxAgent.Core
{
    public sealed class LlmRequest
    {
        public IList<ChatMessage> Messages { get; set; }

        /// <summary>本轮暴露给模型的工具。为空表示纯对话。</summary>
        public IList<ToolSchema> Tools { get; set; }

        public double Temperature { get; set; }

        public int MaxTokens { get; set; }

        public LlmRequest()
        {
            Temperature = 0.2;
            MaxTokens = 4096;
        }
    }

    public sealed class LlmResponse
    {
        public string Content { get; set; }

        /// <summary>推理模型的思考内容（DeepSeek 的 reasoning_content）。普通模型为 null。</summary>
        public string ReasoningContent { get; set; }

        public IList<ToolCall> ToolCalls { get; set; }

        /// <summary>网络/鉴权/限流等调用层失败。区别于模型正常返回。</summary>
        public bool IsError { get; set; }

        public string ErrorMessage { get; set; }

        public int PromptTokens { get; set; }

        public int CompletionTokens { get; set; }

        /// <summary>
        /// true 表示 Content / ReasoningContent 已在生成过程中通过回调逐块发出，
        /// AgentLoop 不会再整段补发一次，避免 UI 重复。
        /// </summary>
        public bool AlreadyStreamed { get; set; }

        public bool HasToolCalls
        {
            get { return ToolCalls != null && ToolCalls.Count > 0; }
        }

        public static LlmResponse Error(string message)
        {
            return new LlmResponse { IsError = true, ErrorMessage = message };
        }
    }

    /// <summary>
    /// 流式增量回调。实现方在收到 SSE 分片时同步调用，调用线程即 HTTP 读取线程，
    /// UI 侧需自行封送。回调可能为 null，实现方须判空。
    /// </summary>
    public sealed class LlmStreamHandlers
    {
        /// <summary>思考内容增量（reasoning_content）。</summary>
        public Action<string> OnReasoningDelta;

        /// <summary>正文增量（content）。</summary>
        public Action<string> OnContentDelta;

        public void Reasoning(string text)
        {
            var h = OnReasoningDelta;
            if (h != null && !string.IsNullOrEmpty(text)) h(text);
        }

        public void Content(string text)
        {
            var h = OnContentDelta;
            if (h != null && !string.IsNullOrEmpty(text)) h(text);
        }
    }

    /// <summary>
    /// LLM 适配契约。DeepSeek / Kimi / 其它各实现一份，
    /// 负责把 ChatMessage + ToolSchema 序列化成各家 API 格式并解析回来。
    /// Core 不引入任何 JSON 库依赖。
    /// </summary>
    public interface ILlmClient
    {
        string ModelId { get; }

        Task<LlmResponse> CompleteAsync(LlmRequest request, CancellationToken ct);
    }

    /// <summary>
    /// 可选的流式扩展。AgentLoop 检测到客户端实现本接口且 SupportsStreaming 为 true 时，
    /// 走 CompleteStreamAsync；否则自动退回 ILlmClient.CompleteAsync。
    /// 因此老客户端不实现本接口也能正常工作。
    /// </summary>
    public interface IStreamingLlmClient : ILlmClient
    {
        /// <summary>运行期能力开关。为 false 时 AgentLoop 不会调用 CompleteStreamAsync。</summary>
        bool SupportsStreaming { get; }

        /// <summary>
        /// 流式生成。实现方须在生成过程中调用 handlers 回调，
        /// 并在返回的 LlmResponse 上把 AlreadyStreamed 置为 true。
        /// 工具调用参数无法可靠地增量呈现，仍在返回值里一次性给出。
        /// </summary>
        Task<LlmResponse> CompleteStreamAsync(LlmRequest request, LlmStreamHandlers handlers, CancellationToken ct);
    }
}
