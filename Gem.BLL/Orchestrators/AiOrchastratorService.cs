using Gem.BLL.Common.Utility;
using Gem.BLL.Interfaces.Orchestrators;
using Gem.BLL.Interfaces.Services;
using Gem.COMMON.Enum;
using Gem.COMMON.ResultModel;
using Gem.COMMON.ViewModel.Messages;
using Gem.COMMON.ViewModel.Prompt;
using Gem.COMMON.ViewModel.Response;
using Gem.COMMON.ViewModel.Thread;
using Gem.COMMON.ViewModel.Token_Usage;
using Gem.DAL.Domain;
using Microsoft.SemanticKernel;

namespace Gem.BLL.Orchestrators
{
    public class AiOrchestratorService(IChatService chatService, IThreadService threadService, IMessageService messageService, ITokenUsageService tokenUsageService) : IAiOrchestratorService
    {
        private readonly IChatService _chatService = chatService;
        private readonly IThreadService _threadService = threadService;
        private readonly IMessageService _messageService = messageService;
        private readonly ITokenUsageService _tokenUsageService = tokenUsageService;

        public async Task<ResModel<VMApiResponse>> HandleAsync(VMPromptRequest request, CancellationToken cancellationToken)
        {
            ValidateRequest(request);

            var threadId = await GetOrCreateThreadAsync(request, cancellationToken);
            if (string.IsNullOrEmpty(threadId))
                return SD.CreateResponse<VMApiResponse>(null, false, "Thread initialization failed", (int)StatusCode.InternalServerError);

            var messages = await _messageService.GetLatestMessagesAsync(threadId, 20, cancellationToken) ?? [];

            var prompt = ResolvePrompt(request);
            await AddUserMessageAsync(threadId, request, prompt, cancellationToken);

            var executionResult = await ExecuteAsync(messages, request, cancellationToken);
            if (!executionResult.Success)
                return SD.CreateResponse<VMApiResponse>(null, false, executionResult.Message, executionResult.StatusCode);

            await SaveAssistantResponseAsync(threadId, request, executionResult, cancellationToken);

            return SD.CreateResponse(new VMApiResponse
            {
                ThreadId = threadId,
                MetaData = executionResult.Data
            }, true, executionResult.Message, executionResult.StatusCode);
        }

        #region Private Helpers

        private static void ValidateRequest(VMPromptRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (string.IsNullOrWhiteSpace(request.Prompt) && (request.Files?.Count ?? 0) == 0)
                throw new ArgumentException("Either prompt or files must be provided");
        }

        private async Task<string?> GetOrCreateThreadAsync(VMPromptRequest request, CancellationToken ct)
        {
            if (!string.IsNullOrWhiteSpace(request.ConversationId))
            {
                var thread = await _threadService.GetThreadAsync(request.ConversationId, ct);
                return thread?.Success == true ? request.ConversationId : null;
            }

            var newThread = await _threadService.AddThreadAsync(new VMAddThread
            {
                Title = request.Prompt ?? "New Thread"
            });

            return newThread?.Success == true ? newThread.Data : null;
        }

        private static string ResolvePrompt(VMPromptRequest request) =>
            !string.IsNullOrWhiteSpace(request.Prompt)? request.Prompt  : (request.Files?.Count > 0 ? "Explain this image" : string.Empty);

        private Task AddUserMessageAsync(string threadId, VMPromptRequest request, string prompt, CancellationToken ct) =>
            _messageService.AddMessageAsync(new VMAddMessage
            {
                ConversationId = threadId,
                Role = ChatRoles.User,
                Content = prompt,
                Model = request.Model,
                Files = request.Files
            }, ct);

        private async Task<ResModel<ChatMessageContent>> ExecuteAsync(List<Message> messages, VMPromptRequest request, CancellationToken ct)
        {
            try
            {
                var chatRes = await _chatService.ExecutePromptAsync(messages, request, ct);
                return MapResponse(chatRes);
            }
            catch (Exception ex)
            {
                return SD.CreateResponse<ChatMessageContent>(null, false, $"Processing failed: {ex.Message}", (int)StatusCode.InternalServerError);
            }
        }

        private static ResModel<ChatMessageContent> MapResponse(dynamic serviceResponse)
        {
            if (serviceResponse is null || serviceResponse.Success is not true)
                return SD.CreateResponse<ChatMessageContent>(null, false, serviceResponse?.Message ?? "Unknown error", serviceResponse?.StatusCode ?? (int)StatusCode.InternalServerError);

            return new ResModel<ChatMessageContent>
            {
                Success = true,
                Data = serviceResponse.Data,
                Message = serviceResponse.Message,
                StatusCode = serviceResponse.StatusCode
            };
        }

        private async Task SaveAssistantResponseAsync(string threadId, VMPromptRequest request, ResModel<ChatMessageContent> result, CancellationToken ct)
        {
            var message = await _messageService.AddMessageAsync(new VMAddMessage
            {
                ConversationId = threadId,
                Role = ChatRoles.Assistant,
                Content = result.Data.Content ?? string.Empty,
                Model = request.Model
            }, ct);

            if (message.Success)
            {
                var metaData = ExtractTokenUsage(result.Data.Metadata);
                metaData.MessageId = message.Data;
                await _tokenUsageService.AddTokenUsageAsync(metaData, ct);
            }
        }

        private static VMAddTokenUsage ExtractTokenUsage(IReadOnlyDictionary<string, object>? metadata)
        {
            if (metadata is null) return new VMAddTokenUsage();

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

        #endregion
    }
}