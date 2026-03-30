using Gem.COMMON.ResultModel;
using Gem.COMMON.ViewModel.Prompt;
using Gem.DAL.Domain;
using Microsoft.SemanticKernel;

namespace Gem.BLL.Interfaces.Services
{
    public interface IChatService
    {
        Task<ResModel<ChatMessageContent>> ExecutePromptAsync(List<Message> messages,VMPromptRequest vMPromptRequest,  CancellationToken cancellationToken = default);
        Task<string> ExecutePromptStreamAsync(VMPromptRequest request, CancellationToken cancellationToken = default);
    }
}
