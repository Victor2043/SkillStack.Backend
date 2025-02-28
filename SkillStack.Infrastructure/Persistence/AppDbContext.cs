using Microsoft.EntityFrameworkCore;
using SkillStack.Domain.Entities;

namespace SkillStack.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {        
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
