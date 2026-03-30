using Gem.COMMON.ResultModel;
using Gem.COMMON.ViewModel.Prompt;
using Gem.COMMON.ViewModel.Response;
namespace Gem.BLL.Interfaces.Orchestrators
{
    public interface IAiOrchestratorService
    {
        Task<ResModel<VMApiResponse>> HandleAsync(VMPromptRequest request, CancellationToken cancellationToken);
    }
}
