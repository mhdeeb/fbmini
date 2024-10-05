using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using fbmini.Server;

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
    }
}
