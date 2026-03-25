using Gem.COMMON.ResultModel;
using Gem.COMMON.Utility;
using Gem.COMMON.ViewModel.Prompt;
using Gem.DAL.Domain;

namespace Gem.BLL.Interfaces.Services
{
    public interface IImageAnalyzerService
    {
        Task<ResModel<string>> AnalyzeAsync(List<Message> messages,List<VMContentInput> vmContentInput, string prompt);
    }
}
