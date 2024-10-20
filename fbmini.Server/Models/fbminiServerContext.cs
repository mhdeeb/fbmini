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

            builder.Entity<UserData>()
                .HasOne(u => u.Picture)
                .WithOne()
                .HasForeignKey<UserData>(u => u.PictureId)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<UserData>()
                .HasOne(u => u.Cover)
                .WithOne()
                .HasForeignKey<UserData>(u => u.CoverId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<UserData> UserData { get; set; }
        public DbSet<FileModel> Files { get; set; }
    }
}
