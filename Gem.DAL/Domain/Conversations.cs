using System.ComponentModel.DataAnnotations.Schema;

namespace Gem.DAL.Domain
{
    public class Conversations
    {
        public string Id { get; private set; } = Guid.NewGuid().ToString();
        public string Title { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser ApplicationUser { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
