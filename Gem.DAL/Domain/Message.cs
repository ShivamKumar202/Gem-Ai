using Gem.COMMON.Enum;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gem.DAL.Domain
{
    public class Message
    {
        public string Id { get; private set; } = Guid.NewGuid().ToString();
        public string Content { get; set; }
        public string Model { get; set; }
        public ChatRoles Role  { get; set; }
        public string ThreadId { get; set; }
        [ForeignKey(nameof(ThreadId))]
        public Thread Thread { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
