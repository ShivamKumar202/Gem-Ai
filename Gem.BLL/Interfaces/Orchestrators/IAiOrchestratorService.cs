using Gem.COMMON.ResultModel;
using Gem.COMMON.ViewModel.Prompt;

namespace Gem.BLL.Interfaces.Orchestrators
{
    public interface IAiOrchestratorService
    {
        Task<ResModel<string>> HandleAsync(VMPromptRequest request, CancellationToken cancellationToken);
    }
}
