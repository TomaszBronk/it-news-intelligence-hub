using ItNewsIntelligenceHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ItNewsIntelligenceHub.Infrastructure.Persistence;

public class NewsHubDbContext(DbContextOptions<NewsHubDbContext> options)
    : DbContext(options)
{
    public DbSet<NewsSource> NewsSources => Set<NewsSource>();
    public DbSet<NewsItem> NewsItems => Set<NewsItem>();

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

        var newsItem = modelBuilder.Entity<NewsItem>();

        newsItem.ToTable("NewsItems");

        newsItem.HasKey(item => item.Id);

        newsItem.Property(item => item.ExternalId)
            .HasMaxLength(512)
            .IsRequired();

        newsItem.Property(item => item.Title)
            .HasMaxLength(500)
            .IsRequired();

        newsItem.Property(item => item.Summary)
            .HasMaxLength(8000);

        newsItem.Property(item => item.OriginalUrl)
            .HasMaxLength(2048)
            .IsRequired();

        newsItem.Property(item => item.Author)
            .HasMaxLength(250);

        newsItem.Property(item => item.ContentHash)
            .HasMaxLength(64)
            .IsRequired();

        newsItem.Property(item => item.Category)
            .HasMaxLength(50)
            .IsRequired();

        newsItem.HasOne(item => item.Source)
            .WithMany(source => source.NewsItems)
            .HasForeignKey(item => item.SourceId)
            .OnDelete(DeleteBehavior.Cascade);

        newsItem.HasIndex(item => new { item.SourceId, item.ExternalId })
            .IsUnique();

        newsItem.HasIndex(item => new { item.SourceId, item.OriginalUrl })
            .IsUnique();

        newsItem.HasIndex(item => item.PublishedAtUtc);
    }
}