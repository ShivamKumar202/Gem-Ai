using Gem.BLL.Common.Utility;
using Gem.BLL.Interfaces.Services;
using Gem.COMMON.Enum;
using Gem.COMMON.ResultModel;
using Gem.COMMON.Utility;
using Gem.COMMON.ViewModel.Conversations;
using Gem.DAL;
using Gem.DAL.Domain;
using Microsoft.EntityFrameworkCore;

namespace Gem.BLL.Services
{
    public class ConversationService(GemContext context, IUserContextService userContextService) : IConversationService
    {
        private readonly GemContext _context = context;
        private readonly IUserContextService _userContextService = userContextService;

        public async Task<ResModel<string>> AddConversationAsync(VMAddConversation vmAddConversation)
        {
            string data = null;
            string userId = _userContextService.UserId;
            if (string.IsNullOrEmpty(vmAddConversation.Title))
                return SD.CreateResponse(data, false, "Title is empty", (int)StatusCode.BadRequest);

            Conversations conversation = new()
            {
                UserId = userId,
                Title = vmAddConversation.Title,
            };
            await _context.Conversations.AddAsync(conversation);
            await _context.SaveChangesAsync();
            return SD.CreateResponse(data, true, "Conversation added successfully", (int)StatusCode.OK);
        }

        public async Task<ResModel> DeleteConversationAsync(string conversationId)
        {
            if (string.IsNullOrEmpty(conversationId))
                return SD.CreateResponse(false, Messages.INVALID_ID, (int)StatusCode.BadRequest);

            var conversation = await _context.Conversations.FirstOrDefaultAsync(x => x.Id == conversationId && !x.DeletedAt.HasValue);
            if (conversation is null)
                return SD.CreateResponse(false, "Conversation not found", (int)StatusCode.BadRequest);

            conversation.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return SD.CreateResponse(true, "Deleted Successfully", (int)StatusCode.OK);
        }

        public async Task<ResModel<VMConversation>> GetConversationAsync(string conversationId, CancellationToken cancellationToken)
        {
            VMConversation vmConversation = null;

            if (string.IsNullOrEmpty(conversationId))
                return SD.CreateResponse(vmConversation, false, Messages.INVALID_ID, (int)StatusCode.BadRequest);

            var conversation = await _context.Conversations.FirstOrDefaultAsync(x => x.Id == conversationId && !x.DeletedAt.HasValue, cancellationToken: cancellationToken);
            if (conversation is null)
                return SD.CreateResponse(vmConversation, false, "Conversation not found", (int)StatusCode.BadRequest);
          
            vmConversation = new()
            {
                Id = conversationId,
                Title = conversation.Title,
                CreatedAt = conversation.CreatedAt,
                UpdatedAt = conversation.UpdatedAt,
            };
            return SD.CreateResponse(vmConversation, true, "Conversation  found", (int)StatusCode.OK);
        }

        public Task<PagingRes<VMConversation>> GetConversationListAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<ResModel> UpdateConversationAsync(string conversationId, DateTime updatedAt)
        {
            if (string.IsNullOrEmpty(conversationId))
                return SD.CreateResponse(false, Messages.INVALID_ID, (int)StatusCode.BadRequest);

            var conversation = await _context.Conversations.FirstOrDefaultAsync(x => x.Id == conversationId && !x.DeletedAt.HasValue);
            if (conversation is null)
                return SD.CreateResponse(false, "Conversation not found", (int)StatusCode.BadRequest);

            conversation.UpdatedAt = updatedAt;
            _context.Conversations.Update(conversation);
            await _context.SaveChangesAsync();

            return SD.CreateResponse(true,"Conversation Updated Successfull",(int)StatusCode.OK);
        }
    }
}
