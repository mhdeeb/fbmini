using Microsoft.EntityFrameworkCore;

namespace fbmini.Server.Data
{
    public class fbminiServerContext : DbContext
    {
        public fbminiServerContext (DbContextOptions<fbminiServerContext> options)
            : base(options)
        {
        }

        public DbSet<fbmini.Server.Test> Test { get; set; } = default!;
        public DbSet<fbmini.Server.DB> DB { get; set; } = default!;
        public DbSet<fbmini.Server.Products> Products { get; set; } = default!;
    }
}
