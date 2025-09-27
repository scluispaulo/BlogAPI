using Microsoft.EntityFrameworkCore;

public class BlogContext : DbContext
{
    public BlogContext(DbContextOptions<BlogContext> options) : base(options) { }

    public DbSet<BlogPost> BlogPosts { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BlogPost>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(250);
            entity.Property(e => e.Content)
                .IsRequired();
            entity.Property(e => e.CreatedAt)
                .IsRequired();
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Author)
                .IsRequired()
                .HasMaxLength(150);
            entity.Property(e => e.Content)
                .IsRequired();
            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.HasOne(e => e.BlogPost)
                  .WithMany(b => b.Comments)
                  .HasForeignKey(e => e.BlogPostId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
