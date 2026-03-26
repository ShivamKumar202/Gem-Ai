using System.ComponentModel.DataAnnotations.Schema;

namespace Gem.DAL.Domain
{
    public class Attachment
    {
        public string Id { get; private set; } = Guid.NewGuid().ToString();
        public string MessageId { get; set; }
        [ForeignKey(nameof(MessageId))]
        public Message Message { get; set; }
        public string FileName { get; set; }
        public string MimeType { get; set; }
        public string Url { get; set; }
        public float[] Embedding { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}