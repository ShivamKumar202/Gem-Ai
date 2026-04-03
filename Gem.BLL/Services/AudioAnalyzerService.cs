using Gem.BLL.Interfaces.Services;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Gem.BLL.Services
{
    public class AudioAnalyzerService(IConfiguration configuration) : IAudioAnalyzerService
    {
        private readonly Client _client = new(apiKey: configuration["GoogleAI:ApiKey"]);
        private readonly string _ttsModel = configuration["GoogleAI:TTSModel"] ?? "gemini-2.5-pro-preview-tts";
        private readonly string _sttModel = configuration["GoogleAI:STTModel"] ?? "gemini-2.5-pro-preview-stt";

        public IReadOnlyDictionary<string, object> Attributes => new Dictionary<string, object>();

        public async Task<IReadOnlyList<AudioContent>> GetAudioContentsAsync( string text, PromptExecutionSettings executionSettings = null,Kernel kernel = null,CancellationToken cancellationToken = default)
        {
            var contents = BuildTextContent(text);

            var config = new GenerateContentConfig
            {
                Temperature = 1,
                ResponseModalities = ["audio"],
                SpeechConfig = new SpeechConfig
                {
                    VoiceConfig = new VoiceConfig
                    {
                        PrebuiltVoiceConfig = new PrebuiltVoiceConfig { VoiceName = "Zephyr" }
                    }
                }
            };

            var response = await _client.Models.GenerateContentAsync(
                model: _ttsModel,
                contents: contents,
                config: config,
                cancellationToken: cancellationToken
            );

            return ExtractAudioParts(response);
        }

        public async Task<string> GetTextFromAudioAsync(List<(byte[] Data, string MimeType)> audio,string prompt, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            var contents = BuildAudioContent(audio);

            if (!string.IsNullOrWhiteSpace(prompt))
            {
                contents.Add(new Content
                {
                    Role = AuthorRole.User.ToString().ToLower(),
                    Parts = [new Part { Text = prompt }]
                });
            }

            var config = new GenerateContentConfig
            {
                Temperature = 0,
                ResponseModalities = ["text"]
            };

            var response = await _client.Models.GenerateContentAsync(
                model: _sttModel,
                contents: contents,
                config: config,
                cancellationToken: cancellationToken
            );

            return ExtractText(response);
        }

        private static List<Content> BuildTextContent(string text) =>
        [
            new() { Role = AuthorRole.User.ToString().ToLower(), Parts = [new() { Text = text }] }
        ];

        private static List<Content> BuildAudioContent(List<(byte[] Data, string MimeType)> audio) =>
          [
            new()
            {
                Role = AuthorRole.User.ToString().ToLower(),
                Parts = [.. audio.Select(a => new Part
                {
                    InlineData = new ()
                    {
                        Data = a.Data,
                        MimeType = a.MimeType
                    }
                })]
            }
          ];


        private static IReadOnlyList<AudioContent> ExtractAudioParts(GenerateContentResponse response) =>
            [.. response.Candidates.SelectMany(c => c.Content.Parts)
                .Where(p => p.InlineData?.Data != null)
                .Select(p => new AudioContent(p.InlineData.Data, p.InlineData.MimeType))];

        private static string ExtractText(GenerateContentResponse response) =>
            response.Candidates.SelectMany(c => c.Content.Parts)
                .Where(p => p.Text != null)
                .Select(p => p.Text)
                .FirstOrDefault() ?? string.Empty;
    }
}