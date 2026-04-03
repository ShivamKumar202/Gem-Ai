using Gem.DAL.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Thread = Gem.DAL.Domain.Thread;

namespace Gem.DAL
{
    public class GemContext(DbContextOptions<GemContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<Attachment> Attachment { get; set; }
        public DbSet<Thread> Thread { get; set; } 
        public DbSet<Token_Usage> TokenUsage { get; set; }
        public DbSet<ExceptionLog> ExceptionLog { get; set; }
        public DbSet<Message> Message { get; set; }
    }
}
