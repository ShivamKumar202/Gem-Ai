using Gem.BLL.Interfaces.Services;
using Gem.BLL.Services;
using Google.GenAI.Types;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace Gem.API.Helpers.Plugins
{
    public class ImagePlugin(IImageAnalyzerService imageService)
    {
        private readonly IImageAnalyzerService _imageService = imageService;

        [KernelFunction("create_visual")]
        [Description("Generate a new image from text. Use when user says 'generate', 'draw', or 'show me'.")]
        public async Task<ImageContent> CreateVisualAsync(string description)
        {
            var images = await _imageService.GenerateImageAsync(description);
            return images[0];
        }

        [KernelFunction("edit_visual")]
        [Description("Edit an existing image with a text prompt, e.g. add/remove objects.")]
        public async Task<Image> EditVisualAsync(List<(byte[] Data, string MimeType)> images, string editPrompt)
        {
           var res=await _imageService.EditImageAsync(images,editPrompt);
            return res[0];
        }

        [KernelFunction("upscale_visual")]
        [Description("Upscale an image to higher resolution.")]
        public async Task<Image> UpscaleVisualAsync(byte[] imageData, string mimeType, string targetSize = "4K")
        {
            var images = await _imageService.UpscaleImageAsync(imageData, mimeType, targetSize);
            return images[0];
        }

        [KernelFunction("describe_visual")]
        [Description("Generate a text description of an image.")]
        public async Task<string> DescribeVisualAsync(byte[] imageData, string mimeType)
        {
            return await _imageService.DescribeImageAsync(imageData, mimeType);
        }
    }
}
