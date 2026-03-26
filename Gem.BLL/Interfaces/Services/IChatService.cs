using Gem.COMMON.Enum;
using Gem.COMMON.ResultModel;
using Gem.COMMON.ViewModel.Prompt;
using Gem.COMMON.ViewModel.Response;
using Gem.DAL.Domain;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Gem.BLL.Interfaces.Services
{
    public interface IChatService
    {
        Task<ResModel<VMApiResponse>> ExecutePromptAsync(List<Message> messages, string prompt,int? maxTokens, double? temperature = 0.7, CancellationToken cancellationToken = default);
        Task<string> ExecutePromptStreamAsync(VMPromptRequest request, CancellationToken cancellationToken = default);
    }
}
