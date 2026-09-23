using ItNewsIntelligenceHub.Server.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ItNewsIntelligenceHub.Server.Infrastructure.Persistence;

public class NewsHubDbContext(DbContextOptions<NewsHubDbContext> options)
    : DbContext(options)
{
    public DbSet<NewsSource> NewsSources => Set<NewsSource>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var newsSource = modelBuilder.Entity<NewsSource>();

        newsSource.ToTable("NewsSources");

        newsSource.HasKey(source => source.Id);

        newsSource.Property(source => source.Name)
            .HasMaxLength(150)
            .IsRequired();

        newsSource.Property(source => source.FeedUrl)
            .HasMaxLength(2048)
            .IsRequired();

        newsSource.Property(source => source.WebsiteUrl)
            .HasMaxLength(2048);

        newsSource.Property(source => source.Category)
            .HasMaxLength(50)
            .IsRequired();

        newsSource.HasIndex(source => source.FeedUrl)
            .IsUnique();
    }
}