using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace fbmini.Server.Models
{
    public class fbminiServerContext : IdentityDbContext<User>
    {
        public fbminiServerContext(DbContextOptions<fbminiServerContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<User> User { get; set; } = default!;
        public DbSet<Test> Test { get; set; } = default!;
        public DbSet<DB> DB { get; set; } = default!;
        public DbSet<Products> Products { get; set; } = default!;
    }
}
