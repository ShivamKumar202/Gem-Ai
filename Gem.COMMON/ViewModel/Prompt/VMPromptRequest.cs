using Microsoft.AspNetCore.Http;
namespace Gem.COMMON.ViewModel.Prompt
{
    public class VMPromptRequest
    {
        public string ConversationId { get; set; }
        public string Prompt { get; set; }
        public int? MaxTokens { get; set; }
        public double? Temperature { get; set; }
        public string Model { get; set; }
        public List<IFormFile> Files { get; set; } = [];
    }
}
