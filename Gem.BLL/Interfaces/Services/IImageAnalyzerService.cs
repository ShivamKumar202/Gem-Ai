using Google.GenAI.Types;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TextToImage;

namespace Gem.BLL.Interfaces.Services
{
    public interface IImageAnalyzerService:ITextToImageService
    {
        Task<IReadOnlyList<ImageContent>> GenerateImageAsync(string prompt, CancellationToken ct = default);
        Task<IReadOnlyList<Image>> UpscaleImageAsync(byte[] imageData, string mimeType, string factor = "x2", CancellationToken ct = default);
        Task<IReadOnlyList<Image>> EditImageAsync(List<(byte[] Data, string MimeType)> images, string editPrompt, CancellationToken ct = default);
        Task<string> DescribeImageAsync(byte[] imageData, string mimeType, CancellationToken ct=default);
    }
}
