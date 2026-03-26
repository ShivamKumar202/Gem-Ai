using Gem.COMMON.ResultModel;
using Gem.COMMON.ViewModel.Response;
using Gem.DAL.Domain;
using Google.GenAI.Types;
using Microsoft.AspNetCore.Http;

namespace Gem.BLL.Interfaces.Services
{
    public interface IImageAnalyzerService
    {
        Task<ResModel<VMApiResponse>> AnalyzeAsync(List<Message> messages, List<IFormFile> files, string prompt, CancellationToken ct = default);
        Task GenerateImageAsync(string prompt, int width = 512, int height = 512, CancellationToken cancellationToken = default);
    }
}
