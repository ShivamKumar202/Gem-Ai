using Gem.COMMON.ResultModel;
using Gem.COMMON.ViewModel.Thread;

namespace Gem.BLL.Interfaces.Services
{
    public interface IThreadService
    {
        Task<ResModel<string>> AddThreadAsync(VMAddThread vmAddThread);
        Task<ResModel<VMThread>> GetThreadAsync(string threadId, CancellationToken cancellationToken);
        Task<ResModel> UpdateThreadAsync(string threadId, DateTime updatedAt);
        Task<PagingRes<VMThread>> GetThreadListAsync();
        Task<ResModel> DeleteThreadAsync(string threadId);
    }
}