using Gem.COMMON.ResultModel;
using Gem.COMMON.ViewModel.Prompt;
using Google.GenAI.Types;
namespace Gem.BLL.Interfaces.Orchestrators
{
    public interface IAiOrchestratorService
    {
        Task<ResModel<string>> HandleAsync(VMPromptRequest request, CancellationToken cancellationToken);
        Task<ResModel> GenerateImageAsync(string prompt,string threadId);
    }
}
