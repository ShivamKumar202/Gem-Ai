using Gem.BLL.IServices;
using Google.GenAI;
using Google.GenAI.Types;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;

namespace Gem.BLL.Services
{
    public class ImageGenerationService(Kernel kernel,IConfiguration configuration) : IImgGenerationService
    {
        private readonly Kernel _kernel = kernel;
        private readonly IConfiguration _configuration = configuration;

        public async Task<Image> GenerateImageAsync(string prompt, int width = 512, int height = 512, CancellationToken cancellationToken = default)
        {
            var section = _configuration.GetRequiredSection("GoogleAI");

            var client = new Client(
                apiKey: section["ApiKey"]
            );
            var generateImagesConfig = new GenerateImagesConfig
            {
                NumberOfImages = 1,
                AspectRatio = "1:1",
                SafetyFilterLevel = SafetyFilterLevel.BlockLowAndAbove,
                PersonGeneration = PersonGeneration.DontAllow,
                IncludeSafetyAttributes = true,
                IncludeRaiReason = true,
                OutputMimeType = "image/jpeg",
            };
            var response = await client.Models.GenerateImagesAsync(model: "imagen-4.0-generate-001", prompt: "Red skateboard", config: generateImagesConfig, cancellationToken: cancellationToken);
            // Do something with the generated image
            var image = response.GeneratedImages.First().Image;
            return image;
        }
    }
}
