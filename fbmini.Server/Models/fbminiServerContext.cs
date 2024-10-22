using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace fbmini.Server.Models
{
    public class fbminiServerContext : IdentityDbContext<User>
    {
        public fbminiServerContext(DbContextOptions<fbminiServerContext> options)
            : base(options)
        {}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PostModel>()
                .HasOne(p => p.Poster)
                .WithMany()
                .HasForeignKey(p => p.PosterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PostModel>()
                .HasMany(p => p.Likers)
                .WithMany(u => u.LikedPosts)
                .UsingEntity(j => j.ToTable("PostLikers"));

            builder.Entity<PostModel>()
                .HasMany(p => p.Dislikers)
                .WithMany(u => u.DislikedPosts)
                .UsingEntity(j => j.ToTable("PostDislikers"));
        }

        public DbSet<UserData> UserData { get; set; }
        public DbSet<FileModel> Files { get; set; }
        public DbSet<PostModel> Posts { get; set; }
    }
}
