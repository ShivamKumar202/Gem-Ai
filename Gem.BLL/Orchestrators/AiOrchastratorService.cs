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
using Google.GenAI.Types;

namespace Gem.BLL.Orchestrators
{
    public class AiOrchestratorService(IChatService chatService, IThreadService threadService, IMessageService messageService, IImageAnalyzerService imageAnalyzerService, ITokenUsageService tokenUsageService) : IAiOrchestratorService
    {
        private readonly IChatService _chatService = chatService;
        private readonly IThreadService _threadService = threadService;
        private readonly IMessageService _messageService = messageService;
        private readonly IImageAnalyzerService _imageAnalyzerService = imageAnalyzerService;
        private readonly ITokenUsageService _tokenUsageService = tokenUsageService;

        public async Task<ResModel<string>> HandleAsync(VMPromptRequest request, CancellationToken cancellationToken)
        {
            ValidateRequest(request);

            var threadId = await GetOrCreateThreadAsync(request, cancellationToken);
            if (string.IsNullOrEmpty(threadId))
                SD.CreateResponse<string>(null, false, $"Thread initialization failed", (int)StatusCode.InternalServerError);

            var messages = await _messageService
                .GetLatestMessagesAsync(threadId, 20, cancellationToken) ?? [];

            var hasFiles = request.Files?.Count > 0;
            var prompt = ResolvePrompt(request, hasFiles);

            await AddUserMessageAsync(threadId, request, prompt, cancellationToken);

            var executionResult = await ExecuteAsync(messages, request, prompt, hasFiles, cancellationToken);
            if (!executionResult.Success)
                return SD.CreateResponse(executionResult.Data.Content,executionResult.Success ,executionResult.Message,executionResult.StatusCode);

            await SaveAssistantResponseAsync(threadId, request, executionResult, cancellationToken);

            return SD.CreateResponse(executionResult.Data.Content, executionResult.Success, executionResult.Message, executionResult.StatusCode);
        }

        public async Task<ResModel> GenerateImageAsync(string prompt, string threadId)
        {
             await _imageAnalyzerService.GenerateImageAsync(prompt);
            return SD.CreateResponse(true, "Image generated SuccessFully", (int)StatusCode.OK);
        }

        #region Private Helpers

        private static void ValidateRequest(VMPromptRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            if ((request.Files == null || request.Files.Count == 0) && string.IsNullOrWhiteSpace(request.Prompt))
                throw new ArgumentException("Either prompt or files must be provided");
        }

        private async Task<string> GetOrCreateThreadAsync(VMPromptRequest request, CancellationToken ct)
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

        private static string ResolvePrompt(VMPromptRequest request, bool hasFiles)
        {
            if (!string.IsNullOrWhiteSpace(request.Prompt))
                return request.Prompt;

            return hasFiles ? "Explain this image" : null;
        }

        private async Task AddUserMessageAsync(string threadId, VMPromptRequest request, string prompt, CancellationToken ct)
        {
            await _messageService.AddMessageAsync(new VMAddMessage
            {
                ConversationId = threadId,
                Role = ChatRoles.User,
                Content = prompt,
                Model = request.Model,
                Files = request.Files
            }, ct);
        }

        private async Task<ResModel<VMApiResponse>> ExecuteAsync(List<Message> messages, VMPromptRequest request, string prompt, bool hasFiles,CancellationToken ct)
        {
            try
            {
                if (hasFiles)
                {
                    var res = await _imageAnalyzerService.AnalyzeAsync(messages, request.Files, prompt);

                    return MapResponse(res);
                }

                var chatRes = await _chatService.ExecutePromptAsync(messages,prompt,request.MaxTokens,request.Temperature,ct);

                return MapResponse(chatRes);
            }
            catch (Exception ex)
            {
               return SD.CreateResponse<VMApiResponse>(null, false, $"Processing failed: {ex.Message}", (int)StatusCode.InternalServerError);
            }
        }

        private static ResModel<VMApiResponse> MapResponse(dynamic serviceResponse)
        {
            if (serviceResponse == null || !serviceResponse.Success)
                return SD.CreateResponse<VMApiResponse>(null, false,serviceResponse?.Message ?? "Unknown error",serviceResponse?.StatusCode ?? (int)StatusCode.InternalServerError);
            

            return new ResModel<VMApiResponse>
            {
                Success = true,
                Data = serviceResponse.Data,
                Message = serviceResponse.Message,
                StatusCode = serviceResponse.StatusCode,
            };
        }

        private async Task SaveAssistantResponseAsync(string threadId,VMPromptRequest request,ResModel<VMApiResponse> result,CancellationToken ct)
        {
            var message = await _messageService.AddMessageAsync(new VMAddMessage
            {
                ConversationId = threadId,
                Role = ChatRoles.Assistant,
                Content = result.Data.Content ?? string.Empty,
                Model = request.Model
            }, ct);

            if (message.Success && result.Data.MetaData is VMAddTokenUsage metaData)
            {
                metaData.MessageId = message.Data;
                await _tokenUsageService.AddTokenUsageAsync(metaData);
            }
        }
        #endregion
    }
}