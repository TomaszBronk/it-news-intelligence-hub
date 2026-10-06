using ItNewsIntelligenceHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ItNewsIntelligenceHub.Infrastructure.Persistence;

public class NewsHubDbContext(DbContextOptions<NewsHubDbContext> options)
    : DbContext(options)
{
    public DbSet<NewsSource> NewsSources => Set<NewsSource>();
    public DbSet<NewsItem> NewsItems => Set<NewsItem>();
    public DbSet<NewsSummary> NewsSummaries => Set<NewsSummary>();

    public DbSet<PostDraft> PostDrafts => Set<PostDraft>();

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

        newsSource.Property(source => source.LastFetchError)
            .HasMaxLength(2000);


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

        newsItem.Property(item => item.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
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

        newsItem.Property(item => item.Note)
    .HasMaxLength(4000);

        var newsSummary = modelBuilder.Entity<NewsSummary>();

        newsSummary.ToTable("NewsSummaries");

        newsSummary.HasKey(s => s.Id);

        newsSummary.Property(s => s.Id)
            .IsRequired();

        newsSummary.Property(s => s.NewsItemId)
            .IsRequired();

        newsSummary.HasIndex(s => s.NewsItemId);

        newsSummary.Property(s => s.Content)
            .IsRequired()
            .HasMaxLength(4000);

        newsSummary.Property(s => s.Model)
            .IsRequired()
            .HasMaxLength(100);

        newsSummary.Property(s => s.CreatedAtUtc)
            .IsRequired();


        var postDraft = modelBuilder.Entity<PostDraft>();

        postDraft.ToTable("PostDrafts");

        postDraft.HasKey(d => d.Id);

        postDraft.Property(d => d.Id)
            .IsRequired();

        postDraft.Property(d => d.NewsItemId)
            .IsRequired();

        postDraft.HasIndex(d => d.NewsItemId);

        postDraft.Property(d => d.Title)
            .IsRequired()
            .HasMaxLength(500);

        postDraft.Property(d => d.Content)
            .IsRequired()
            .HasMaxLength(4000);

        postDraft.Property(d => d.Type)
            .IsRequired()
            .HasMaxLength(50);

        postDraft.Property(d => d.SourceUrl)
            .IsRequired()
            .HasMaxLength(2000);

        postDraft.Property(d => d.IsPublished)
            .IsRequired();

        postDraft.Property(d => d.CreatedAtUtc)
            .IsRequired();

        postDraft.Property(d => d.UpdatedAtUtc)
            .IsRequired(false);
    }
}