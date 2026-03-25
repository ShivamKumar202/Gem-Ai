using System.ComponentModel.DataAnnotations.Schema;

namespace Gem.DAL.Domain
{
    public class Conversations
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser ApplicationUser { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
