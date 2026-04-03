using Gem.BLL.Interfaces.Services;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace Gem.API.Helpers.Plugins
{
    public class AudioPlugin(IAudioAnalyzerService audioAnalyzerService)
    {
        private readonly IAudioAnalyzerService _audioAnalyzerService = audioAnalyzerService;
        [KernelFunction("Speak")]
        [Description("Generate a audio from text. Use when user says 'generate', 'speak', or 'tell me'.")]
        public async Task<IReadOnlyList<AudioContent>> CreateAudioAsync(string description)
        {
            return await _audioAnalyzerService.GetAudioContentsAsync(description);
        }

        [KernelFunction("Transcribe")]
        [Description("Transcribe a text from audio,or particualr line from audio etc.")]
        public async Task<string> CreateTextAsync(List<(byte[] Data, string MimeType)> audio, string prompt)
        {
            return await _audioAnalyzerService.GetTextFromAudioAsync(audio, prompt);
        }
    }
}
