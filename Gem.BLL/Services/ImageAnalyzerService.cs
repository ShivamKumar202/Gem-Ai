using Gem.BLL.Interfaces.Services;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Gem.BLL.Services
{
    public class ImageAnalyzerService(IConfiguration configuration) : IImageAnalyzerService
    {
        private readonly Client _client = new(apiKey: configuration["GoogleAI:ApiKey"]);
        private readonly string _generateModel = configuration["GoogleAI:Model"];
        private readonly string _upscaleModel = configuration["GoogleAI:Model"];
        private readonly string _editModel = configuration["GoogleAI:Model"];

        public IReadOnlyDictionary<string, object> Attributes => new Dictionary<string, object>();

        // Generate
        public async Task<IReadOnlyList<ImageContent>> GenerateImageAsync(string prompt, CancellationToken ct = default)
        {
            var config = new GenerateImagesConfig
            {
                NumberOfImages = 1,
                AspectRatio = "1:1",
                SafetyFilterLevel = SafetyFilterLevel.BlockLowAndAbove,
                PersonGeneration = PersonGeneration.DontAllow,
                IncludeSafetyAttributes = true,
                IncludeRaiReason = true,
                OutputMimeType = "image/jpeg",
            };

            var response = await _client.Models.GenerateImagesAsync(_generateModel, prompt, config, ct);
            return response.GeneratedImages .Select(img => new ImageContent(img.Image.ImageBytes, img.Image.MimeType)) .ToList();

        }

        // Upscale
        public async Task<IReadOnlyList<Image>> UpscaleImageAsync(byte[] imageData, string mimeType, string factor = "x2", CancellationToken ct = default)
        {
            var config = new UpscaleImageConfig { OutputMimeType = mimeType, EnhanceInputImage = true };
            Image image = new()
            {
                MimeType = mimeType,
                ImageBytes = imageData,
            };

            var response = await _client.Models.UpscaleImageAsync(_upscaleModel, image, factor, config, ct);
            return [.. response.GeneratedImages.Select(x=>x.Image)];
        }

        // Edit
        public async Task<IReadOnlyList<Image>> EditImageAsync(List<(byte[] Data, string MimeType)> images, string editPrompt, CancellationToken ct = default)
        {
            var referenceImages = new List<IReferenceImage>();

            int id = 1;
            foreach (var (data, mime) in images)
            {
                var rawReferenceImage = new RawReferenceImage
                {
                    ReferenceImage = new Image
                    {
                        ImageBytes = data,
                        MimeType = mime
                    },
                    ReferenceId = id
                };
                referenceImages.Add(rawReferenceImage);
                id++;
            }
            var editConfig = new EditImageConfig
            {
                EditMode = EditMode.EditModeInpaintInsertion,
                NumberOfImages = images.Count,
                OutputMimeType = "image/jpeg"
            };

            var response = await _client.Models.EditImageAsync(_editModel, editPrompt, referenceImages,cancellationToken: ct);
            return [.. response.GeneratedImages.Select(x => x.Image)];
        }

        // Describe (text output)
        public async Task<string> DescribeImageAsync(byte[] imageData, string mimeType, CancellationToken ct = default)
        {
            var contents = new List<Content>
            {
                new() { Role = AuthorRole.User.ToString(), Parts = [
                    new Part { Text = "Describe this image" },
                    new Part { InlineData = new () { Data = imageData, MimeType = mimeType } }
                ]}
            };

            var config = new GenerateContentConfig { ResponseModalities = ["TEXT"] };
            var response = await _client.Models.GenerateContentAsync(_editModel, contents, config, ct);

            return response.Candidates.SelectMany(c => c.Content.Parts).FirstOrDefault(p => !string.IsNullOrWhiteSpace(p.Text))?.Text ?? "No description generated.";
        }

        // Required by ITextToImageService
        public async Task<IReadOnlyList<ImageContent>> GetImageContentsAsync(TextContent input, PromptExecutionSettings executionSettings = null, Kernel kernel = null, CancellationToken cancellationToken = default)
        {
            return await GenerateImageAsync(input.Text, cancellationToken);
        }
    }

}
