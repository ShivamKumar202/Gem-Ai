using Gem.COMMON.ResultModel;
using Gem.COMMON.ViewModel.Conversations;

namespace Gem.BLL.Interfaces.Services
{
    public interface IConversationService
    {
        Task<ResModel<string>> AddConversationAsync(VMAddConversation vmAddConversation);
        Task<ResModel<VMConversation>> GetConversationAsync(string conversationId, CancellationToken cancellationToken);
        Task<ResModel> UpdateConversationAsync(string conversationId,DateTime updatedAt);
        Task<PagingRes<VMConversation>> GetConversationListAsync();
        Task<ResModel> DeleteConversationAsync(string conversationId);
    }
}