using Gem.COMMON.ViewModel.Chat;
using Microsoft.SemanticKernel;

namespace Gem.BLL.IServices
{
    public interface IChatService
    {
        Task<string> ExecutePromptAsync(VMChat request, CancellationToken cancellationToken = default);
        Task<string> ExecutePromptStreamAsync(VMChat request, CancellationToken cancellationToken = default);
    }
}
