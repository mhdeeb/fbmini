using Microsoft.EntityFrameworkCore;

namespace fbmini.Server.Models
{
    public class fbminiServerContext : DbContext
    {
        public fbminiServerContext(DbContextOptions<fbminiServerContext> options)
            : base(options)
        {
        }

        public DbSet<Test> Test { get; set; } = default!;
        public DbSet<DB> DB { get; set; } = default!;
        public DbSet<Products> Products { get; set; } = default!;
    }
}
