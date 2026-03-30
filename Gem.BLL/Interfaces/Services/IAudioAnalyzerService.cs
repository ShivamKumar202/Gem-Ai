using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.TextToAudio;

namespace Gem.BLL.Interfaces.Services
{
    public interface IAudioAnalyzerService : ITextToAudioService
    {
        Task<string> GetTextFromAudioAsync(List<(byte[] Data, string MimeType)> audio,string prompt, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default);
    }
}
