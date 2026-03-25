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
        public List<VMContentInput> ContentInput { get; set; }
    }
    public class VMContentInput
    {
        public byte[] Data { get; set; } = default!;
        public string MimeType { get; set; } = default!;
    }
}
