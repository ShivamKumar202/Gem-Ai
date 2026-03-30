using Gem.BLL.Common.Utility;
using Gem.BLL.Interfaces.Services;
using Gem.COMMON.Enum;
using Gem.COMMON.ResultModel;
using Gem.COMMON.ViewModel.Prompt;
using Gem.DAL.Domain;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using System.Diagnostics;

namespace Gem.BLL.Services
{
    public class ChatService(Kernel kernel) : IChatService
    {
        private readonly IChatCompletionService _chatService = kernel.GetRequiredService<IChatCompletionService>();

        public async Task<ResModel<ChatMessageContent>> ExecutePromptAsync( List<Message> messages, VMPromptRequest request,CancellationToken cancellationToken = default)
        {
            try
            {
                var chatHistory = BuildChatHistory(messages, request.Prompt);
                var userMessage = await BuildUserMessageAsync(request, cancellationToken).ConfigureAwait(false);

                chatHistory.Add(userMessage);

                var settings = new GeminiPromptExecutionSettings
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
                };

                var stopwatch = Stopwatch.StartNew();
                var response = await _chatService.GetChatMessageContentAsync(chatHistory, settings, kernel, cancellationToken).ConfigureAwait(false);
                stopwatch.Stop();

                return SD.CreateResponse(response, true, $"Prompt successful ({stopwatch.ElapsedMilliseconds} ms)", (int)StatusCode.OK);
            }
            catch (OperationCanceledException)
            {
                return SD.CreateResponse<ChatMessageContent>(null, false, "Request cancelled", (int)StatusCode.RequestTimeout);
            }
            catch (Exception ex)
            {
                return SD.CreateResponse<ChatMessageContent>(null, false, ex.Message, (int)StatusCode.InternalServerError);
            }
        }

        public async Task<string> ExecutePromptStreamAsync(VMPromptRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var chatHistory = BuildChatHistory([], request.Prompt);
                var userMessage = await BuildUserMessageAsync(request, cancellationToken).ConfigureAwait(false);

                chatHistory.Add(userMessage);

                var settings = new GeminiPromptExecutionSettings
                {
                    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
                };

                await foreach (var chunk in _chatService.GetStreamingChatMessageContentsAsync(chatHistory, settings, kernel, cancellationToken).ConfigureAwait(false))
                {
                    if (!string.IsNullOrWhiteSpace(chunk.Content))
                        return chunk.Content; 
                }

                return string.Empty;
            }
            catch (OperationCanceledException)
            {
                return "Request cancelled";
            }
            catch (Exception ex)
            {
                return $"Streaming failed: {ex.Message}";
            }
        }

        #region Private Helpers

        private static ChatHistory BuildChatHistory(List<Message> messages, string prompt)
        {
            var chatHistory = new ChatHistory();

            if (messages is { Count: > 0 })
            {
                foreach (var msg in messages)
                {
                    if (msg.Role == ChatRoles.User)
                        chatHistory.AddUserMessage(msg.Content);
                    else if (msg.Role == ChatRoles.Assistant)
                        chatHistory.AddAssistantMessage(msg.Content);
                }
            }

            if (!string.IsNullOrWhiteSpace(prompt))
                chatHistory.AddUserMessage(prompt);

            return chatHistory;
        }

        private static async Task<ChatMessageContent> BuildUserMessageAsync(VMPromptRequest request, CancellationToken ct)
        {
            var message = new ChatMessageContent(AuthorRole.User, request.Prompt);

            if (request.Files is { Count: > 0 })
            {
                foreach (var file in request.Files)
                {
                    using var ms = new MemoryStream();
                    await file.CopyToAsync(ms, ct).ConfigureAwait(false);
                    var data = ms.ToArray();

                    var contentType = file.ContentType.ToLowerInvariant();

                    message.Items.Add(contentType switch
                    {
                        var type when type.StartsWith("image/") => new ImageContent(data, contentType),
                        var type when type.StartsWith("video/") => new BinaryContent(data, contentType),
                        var type when type.StartsWith("audio/") => new AudioContent(data, contentType),
                        _ => throw new NotSupportedException($"Unsupported file type: {contentType}")
                    });
                }
            }

            return message;
        }
        #endregion
    }
}