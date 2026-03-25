using Gem.BLL.Interfaces.Services;
using Gem.COMMON.ResultModel;
using Gem.COMMON.ViewModel.Messages;
using Gem.DAL;
using Gem.DAL.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gem.BLL.Services
{
    public class MessageService(GemContext context) : IMessageService
    {
        private readonly GemContext _context = context;

        public Task<ResModel> AddMessageAsync(VMAddMessage vmAddMessage, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ResModel> DeleteMessageAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<Message>> GetLatestMessagesAsync(string conversationId, int size, CancellationToken cancellationToken) =>
        await _context.Message.AsNoTracking().Where(m => m.ConversationId == conversationId).OrderByDescending(m => m.CreatedAt)
        .Take(size).OrderBy(m => m.CreatedAt).ToListAsync(cancellationToken);
    }
}
