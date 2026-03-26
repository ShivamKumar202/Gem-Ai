using Gem.COMMON.Enum;
using Microsoft.AspNetCore.Identity;

namespace Gem.DAL.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public UserStatus Status { get; set; }
        public string ProfileUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
