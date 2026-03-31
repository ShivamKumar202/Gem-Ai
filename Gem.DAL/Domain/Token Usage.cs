using System.ComponentModel.DataAnnotations.Schema;

namespace Gem.DAL.Domain
{
    public class Token_Usage
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string MessageId { get; set; }
        [ForeignKey(nameof(MessageId))]
        public Message Message { get; set; }
        public int? CachedContentTokenCount { get; set; }
        public int? CandidatesTokenCount { get; set; }
        public int? PromptTokenCount { get; set; }
        public int? ThoughtsTokenCount { get; set; }
        public int? ToolUsePromptTokenCount { get; set; }
        public int? TotalTokenCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
