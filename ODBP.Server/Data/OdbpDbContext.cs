using Microsoft.EntityFrameworkCore;
using ODBP.Data.Entities;

namespace ODBP.Data
{
    public class OdbpDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Resources> Resources { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Resources>().HasData(new Resources { Id = 1 });
        }
    }
}
