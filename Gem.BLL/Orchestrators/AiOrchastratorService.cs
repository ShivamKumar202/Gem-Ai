using Gem.BLL.Common.Utility;
using Gem.BLL.Interfaces.Orchestrators;
using Gem.BLL.Interfaces.Services;
using Gem.COMMON.Enum;
using Gem.COMMON.ResultModel;
using Gem.COMMON.ViewModel.Messages;
using Gem.COMMON.ViewModel.Prompt;
using Gem.DAL.Domain;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Gem.BLL.Orchestrators
{
    public class AiOrchestratorService(IChatService chatService, IConversationService conversationService, IMessageService messageService, IImageAnalyzerService imageAnalyzerService) : IAiOrchestratorService
    {
        private readonly IChatService _chatService = chatService;
        private readonly IConversationService _conversationService = conversationService;
        private readonly IMessageService _messageService = messageService;
        private readonly IImageAnalyzerService _imageAnalyzerService = imageAnalyzerService;

        public async Task<ResModel<string>> HandleAsync(VMPromptRequest request, CancellationToken cancellationToken)
        {
            List<Message> messages = [];
            if ((request.ContentInput == null || request.ContentInput.Count == 0) && string.IsNullOrWhiteSpace(request.Prompt))
                throw new ArgumentException("Either prompt or content must be provided");

            var conversation = await _conversationService.GetConversationAsync(request.ConversationId, cancellationToken);

            if (conversation.Success)
                messages = await _messageService.GetLatestMessagesAsync(conversation.Data.Id, 20, cancellationToken);

            var chatHistory = new ChatHistory();

            foreach (var msg in messages.OrderBy(m => m.CreatedAt))
            {
                if (msg.Role == ChatRoles.User)
                    chatHistory.AddUserMessage(msg.Content);

                else if (msg.Role == ChatRoles.Assistant)
                    chatHistory.AddAssistantMessage(msg.Content);
            }


            // Case A: multimodal (text + files)
            if (request.ContentInput != null && request.ContentInput.Count > 0
                && !string.IsNullOrWhiteSpace(request.Prompt))
            {
                await _messageService.AddMessageAsync(new VMAddMessage()
                {
                    ConversationId = conversation.Data.Id,
                    Role = ChatRoles.User,
                    Content = request.Prompt,
                    Model = request.Model
                }, cancellationToken);

                var result = await _imageAnalyzerService.AnalyzeAsync(messages, request.ContentInput, request.Prompt);

                await _messageService.AddMessageAsync(new VMAddMessage()
                {
                    ConversationId = conversation.Data.Id,
                    Role = ChatRoles.Assistant,
                    Content = result.Data,
                    Model = request.Model
                }, cancellationToken);

                return result;
            }

            if (request.ContentInput != null && request.ContentInput.Count > 0)
            {
                await _messageService.AddMessageAsync(
                    new VMAddMessage()
                    {
                        ConversationId = conversation.Data.Id,
                        Role = ChatRoles.User,
                        Content = request.Prompt ?? "Explain this image",
                        Model = request.Model
                    }, cancellationToken);

                var result = await _imageAnalyzerService.AnalyzeAsync(messages, request.ContentInput, "Explain this image");

                await _messageService.AddMessageAsync(
               new VMAddMessage()
                   {
                       ConversationId = conversation.Data.Id,
                       Role = ChatRoles.Assistant,
                       Content = request.Prompt,
                       Model = request.Model
                   }, cancellationToken);
                return result;
            }

            if (!string.IsNullOrWhiteSpace(request.Prompt))
            {
                await _messageService.AddMessageAsync(
                    new VMAddMessage()
                    {
                        ConversationId = conversation.Data.Id,
                        Role = ChatRoles.User,
                        Content = request.Prompt ,
                        Model = request.Model
                    }, cancellationToken);

                var result = await _chatService.ExecutePromptAsync(messages,request.Prompt,request.MaxTokens,request.Temperature,cancellationToken);

                await _messageService.AddMessageAsync(
                   new VMAddMessage()
                   {
                       ConversationId = conversation.Data.Id,
                       Role = ChatRoles.Assistant,
                       Content = result.Data,
                       Model = request.Model
                   }, cancellationToken);

                return result;
            }
            return SD.CreateResponse<string>(null, false, " ", (int)StatusCode.BadRequest);
        }
    }
}
