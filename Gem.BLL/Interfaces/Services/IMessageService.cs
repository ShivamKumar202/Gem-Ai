using Gem.COMMON.ResultModel;
using Gem.COMMON.ViewModel.Messages;
using Gem.DAL.Domain;

namespace Gem.BLL.Interfaces.Services
{
    public interface IMessageService
    {
        Task<ResModel> AddMessageAsync(VMAddMessage vmAddMessage,CancellationToken cancellationToken =default);
        Task<ResModel> DeleteMessageAsync();
        Task<List<Message>> GetLatestMessagesAsync(string conversationId,int size, CancellationToken cancellationToken);
        //Task<ResListModel<>> GetMessages(string conversationId);
    }
}
