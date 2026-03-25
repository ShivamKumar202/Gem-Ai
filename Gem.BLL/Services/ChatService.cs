using Azure.Core;
using Gem.BLL.Common.Utility;
using Gem.BLL.Interfaces.Services;
using Gem.COMMON.Enum;
using Gem.COMMON.ResultModel;
using Gem.COMMON.Utility;
using Gem.COMMON.ViewModel.Prompt;
using Gem.DAL;
using Gem.DAL.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Google;
using System.Diagnostics;

namespace Gem.BLL.Services
{
    public class ChatService(Kernel kernel) : IChatService
    {
        private readonly Kernel _kernel = kernel;

        public async Task<ResModel<string>> ExecutePromptAsync(List<Message> messages, string prompt, int? maxTokens, double? temperature = 0.7, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                throw new ArgumentException("Prompt cannot be empty");

            var chatHistory = new ChatHistory();
            foreach (var msg in messages)
            {
                if (msg.Role == ChatRoles.User)
                    chatHistory.AddUserMessage(msg.Content);

                else if (msg.Role == ChatRoles.Assistant)
                    chatHistory.AddAssistantMessage(msg.Content);
            }

            chatHistory.AddUserMessage(prompt);

            var stopwatch = Stopwatch.StartNew();

            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();


            var executionSettings = new GeminiPromptExecutionSettings
            {
                Temperature = temperature,
                MaxTokens = maxTokens
            };

            var result = await chatCompletionService.GetChatMessageContentAsync(chatHistory, executionSettings, cancellationToken: cancellationToken);

            var response = result.Content ?? string.Empty;


            stopwatch.Stop();
            return SD.CreateResponse(response, true, "prompt successfull", (int)StatusCode.OK);
        }

        public Task<string> ExecutePromptStreamAsync(VMPromptRequest request, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}