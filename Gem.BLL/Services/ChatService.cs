using Gem.BLL.Common.Utility;
using Gem.BLL.Interfaces.Services;
using Gem.COMMON.Enum;
using Gem.COMMON.ResultModel;
using Gem.COMMON.ViewModel.Prompt;
using Gem.COMMON.ViewModel.Response;
using Gem.COMMON.ViewModel.Token_Usage;
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

        public async Task<ResModel<VMApiResponse>> ExecutePromptAsync(List<Message> messages, string prompt, int? maxTokens,  double? temperature = 0.7, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                return SD.CreateResponse<VMApiResponse>(null, false, "Prompt cannot be empty", (int)StatusCode.BadRequest);

            try
            {
                var chatHistory = BuildChatHistory(messages, prompt);

                var executionSettings = new GeminiPromptExecutionSettings
                {
                    Temperature = temperature,
                    MaxTokens = maxTokens
                };

                var stopwatch = Stopwatch.StartNew();

                var result = await _chatService.GetChatMessageContentAsync(
                    chatHistory,
                    executionSettings,
                    cancellationToken: cancellationToken);

                stopwatch.Stop();

                var vmResponse = new VMApiResponse
                {
                    Content = result?.Content ?? string.Empty,
                    MetaData = ExtractTokenUsage(result?.Metadata)
                };

                return SD.CreateResponse( vmResponse, true,$"Prompt successful ({stopwatch.ElapsedMilliseconds} ms)",(int)StatusCode.OK);
            }
            catch (OperationCanceledException)
            {
                return SD.CreateResponse<VMApiResponse>(null, false, "Request cancelled", (int)StatusCode.RequestTimeout);
            }
            catch (Exception ex)
            {
                return SD.CreateResponse<VMApiResponse>(null, false, ex.Message, (int)StatusCode.InternalServerError);
            }
        }

        private static ChatHistory BuildChatHistory(List<Message> messages, string prompt)
        {
            var chatHistory = new ChatHistory();

            if (messages != null)
            {
                foreach (var msg in messages)
                {
                    if (msg.Role == ChatRoles.User)
                        chatHistory.AddUserMessage(msg.Content);

                    else if (msg.Role == ChatRoles.Assistant)
                        chatHistory.AddAssistantMessage(msg.Content);
                }
            }

            chatHistory.AddUserMessage(prompt);
            return chatHistory;
        }

        private static VMAddTokenUsage ExtractTokenUsage(IReadOnlyDictionary<string, object> metadata)
        {
            if (metadata == null)
                return new VMAddTokenUsage();

            int GetValue(string key) => metadata.TryGetValue(key, out var val) && val is int i ? i : 0;

            return new VMAddTokenUsage
            {
                PromptTokenCount = GetValue("PromptTokenCount"),
                CachedContentTokenCount = GetValue("CachedContentTokenCount"),
                CandidatesTokenCount = GetValue("CandidatesTokenCount"),
                ThoughtsTokenCount = GetValue("ThoughtsTokenCount"),
                TotalTokenCount = GetValue("TotalTokenCount")
            };
        }

        public Task<string> ExecutePromptStreamAsync(VMPromptRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}