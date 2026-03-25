using Gem.DAL.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gem.DAL
{
    public class GemContext(DbContextOptions<GemContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<Conversations> Conversations { get; set; } 
        public DbSet<ExceptionLog> ExceptionLog { get; set; }
        public DbSet<Message> Message { get; set; }
    }
}
