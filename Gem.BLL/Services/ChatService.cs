using Gem.BLL.IServices;
using Gem.COMMON.ViewModel.Chat;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Gem.BLL.Services
{
    public class ChatService(Kernel kernel) : IChatService
    {
        private readonly Kernel _kernel = kernel;
        private static readonly ConcurrentDictionary<string, ChatHistory> _userChats  = new ConcurrentDictionary<string, ChatHistory>();

        public async Task<string> ExecutePromptAsync(VMChat request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
                throw new ArgumentException("Prompt cannot be empty", nameof(request));

            var userId = string.Empty;

            if (string.IsNullOrWhiteSpace(userId))
                userId = request.SessionId;


            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("UserId or SessionId is required");


            var stopwatch = Stopwatch.StartNew();

            try
            {
                // Thread-safe history per user
                var chatHistory = GetOrCreateHistory(request.SessionId);

                // Add user message
                chatHistory.AddUserMessage(request.Prompt);

                var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

                var executionSettings = new GeminiPromptExecutionSettings
                {
                    Temperature = request.Temperature,
                    MaxTokens = request.MaxTokens
                };

                var result = await chatCompletionService.GetChatMessageContentAsync(
                    chatHistory,
                    executionSettings,
                    cancellationToken: cancellationToken
                );

                var response = result.Content ?? string.Empty;

                chatHistory.AddAssistantMessage(response);

                TrimHistory(chatHistory, 20);

                stopwatch.Stop();

                return response;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                throw new InvalidOperationException($"Error executing prompt: {ex.Message}", ex);
            }
        }
        public Task<string> ExecutePromptStreamAsync(VMChat request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }


        private void TrimHistory(ChatHistory history, int maxMessages)
        {
            while (history.Count > maxMessages)
            {
                history.RemoveAt(0);
            }
        }

        private static readonly ConcurrentDictionary<string, object> _locks = new();

        private object GetUserLock(string userId)
        {
            return _locks.GetOrAdd(userId, _ => new object());
        }
        private ChatHistory GetOrCreateHistory(string userId)
        {
            return _userChats.GetOrAdd(userId, _ => new ChatHistory());
        }
    }

}
