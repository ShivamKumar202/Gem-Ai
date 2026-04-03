using Gem.BLL.Common.Utility;
using Gem.BLL.Interfaces.Services;
using Gem.COMMON.Enum;
using Gem.COMMON.ResultModel;
using Gem.COMMON.Utility;
using Gem.COMMON.ViewModel.Thread;
using Gem.DAL;
using Gem.DAL.Domain;
using Microsoft.EntityFrameworkCore;

namespace Gem.BLL.Services
{
    public class ThreadService(GemContext context, IUserContextService userContextService) : IThreadService
    {
        private readonly GemContext _context = context;
        private readonly IUserContextService _userContextService = userContextService;

        public async Task<ResModel<string>> AddThreadAsync(VMAddThread vmAddThread)
        {
            string data = null;
            string userId = _userContextService.UserId;
            if (string.IsNullOrEmpty(vmAddThread.Title))
                return SD.CreateResponse(data, false, "Title is empty", (int)StatusCode.BadRequest);

            DAL.Domain.Thread conversation = new()
            {
                UserId = userId,
                Title = vmAddThread.Title,
            };
            await _context.Thread.AddAsync(conversation);
            await _context.SaveChangesAsync();
            data = conversation.Id;
            return SD.CreateResponse(data, true, "Thread added successfully", (int)StatusCode.OK);
        }

        public async Task<ResModel> DeleteThreadAsync(string threadId)
        {
            if (string.IsNullOrEmpty(threadId))
                return SD.CreateResponse(false, Messages.INVALID_ID, (int)StatusCode.BadRequest);

            var conversation = await _context.Thread.FirstOrDefaultAsync(x => x.Id == threadId && !x.DeletedAt.HasValue);
            if (conversation is null)
                return SD.CreateResponse(false, "Thread not found", (int)StatusCode.BadRequest);

            conversation.DeletedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return SD.CreateResponse(true, "Deleted Successfully", (int)StatusCode.OK);
        }

        public async Task<ResModel<VMThread>> GetThreadAsync(string threadId, CancellationToken cancellationToken)
        {
            VMThread vmThread = null;

            if (string.IsNullOrEmpty(threadId))
                return SD.CreateResponse(vmThread, false, Messages.INVALID_ID, (int)StatusCode.BadRequest);

            var conversation = await _context.Thread.FirstOrDefaultAsync(x => x.Id == threadId && !x.DeletedAt.HasValue, cancellationToken: cancellationToken);
            if (conversation is null)
                return SD.CreateResponse(vmThread, false, "Thread not found", (int)StatusCode.BadRequest);

            vmThread = new()
            {
                Id = threadId,
                Title = conversation.Title,
                CreatedAt = conversation.CreatedAt,
                UpdatedAt = conversation.UpdatedAt,
            };
            return SD.CreateResponse(vmThread, true, "Thread  found", (int)StatusCode.OK);
        }

        public Task<PagingRes<VMThread>> GetThreadListAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<ResModel> UpdateThreadAsync(string conversationId, DateTime updatedAt)
        {
            if (string.IsNullOrEmpty(conversationId))
                return SD.CreateResponse(false, Messages.INVALID_ID, (int)StatusCode.BadRequest);

            var conversation = await _context.Thread.FirstOrDefaultAsync(x => x.Id == conversationId && !x.DeletedAt.HasValue);
            if (conversation is null)
                return SD.CreateResponse(false, "Thread not found", (int)StatusCode.BadRequest);

            conversation.UpdatedAt = updatedAt;
            _context.Thread.Update(conversation);
            await _context.SaveChangesAsync();

            return SD.CreateResponse(true,"Thread Updated Successfull",(int)StatusCode.OK);
        }
    }
}
