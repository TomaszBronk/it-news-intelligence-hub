using System.ServiceModel.Syndication;
using System.Xml;
using ItNewsIntelligenceHub.Application.Abstractions.Feeds;
using ItNewsIntelligenceHub.Application.NewsSources.Commands.FetchNewsSource;
using Microsoft.Extensions.Logging;


namespace ItNewsIntelligenceHub.Infrastructure.Feeds;

    public class RssFeedReader(
    HttpClient httpClient,
    ILogger<RssFeedReader> logger) : IRssFeedReader
    {
        public async Task<IReadOnlyCollection<FeedNewsItem>> ReadAsync(
            Uri feedUrl,
            CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, feedUrl);

            request.Headers.UserAgent.ParseAdd(
                "ITNewsIntelligenceHub/1.0 (+https://github.com/TomaszBronk/it-news-intelligence-hub)");

            using var response = await httpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

            using var xmlReader = XmlReader.Create(
                stream,
                new XmlReaderSettings
                {
                    Async = true,
                    DtdProcessing = DtdProcessing.Prohibit,
                    IgnoreComments = true,
                    IgnoreWhitespace = true,
                    MaxCharactersInDocument = 5_000_000
                });

            var feed = SyndicationFeed.Load(xmlReader)
                ?? throw new InvalidOperationException(
                    $"Unable to load the RSS/Atom feed from '{feedUrl}'.");

            var feedItems = new List<FeedNewsItem>();

            foreach (var item in feed.Items)
            {
                var originalUrl = item.Links
                    .FirstOrDefault(link => link.RelationshipType is null or "alternate")
                    ?.Uri?
                    .AbsoluteUri;

                if (string.IsNullOrWhiteSpace(originalUrl))
                {
                    logger.LogWarning(
                        "Skipping feed item without an original URL from {FeedUrl}. Title: {Title}",
                        feedUrl,
                        item.Title?.Text);

                    continue;
                }

                var title = item.Title?.Text?.Trim();

                if (string.IsNullOrWhiteSpace(title))
                {
                    logger.LogWarning(
                        "Skipping feed item without a title from {FeedUrl}. URL: {OriginalUrl}",
                        feedUrl,
                        originalUrl);

                    continue;
                }

                var externalId = item.Id?.Trim();

                if (string.IsNullOrWhiteSpace(externalId))
                {
                    externalId = originalUrl;
                }

                var summary = item.Summary?.Text?.Trim();

                var author = item.Authors
                    .FirstOrDefault()?
                    .Name?
                    .Trim();

                DateTimeOffset? publishedAtUtc = null;

                if (item.PublishDate != default)
                {
                    publishedAtUtc = item.PublishDate.ToUniversalTime();
                }
                else if (item.LastUpdatedTime != default)
                {
                    publishedAtUtc = item.LastUpdatedTime.ToUniversalTime();
                }

                feedItems.Add(
                    new FeedNewsItem(
                        externalId,
                        title,
                        summary,
                        originalUrl,
                        author,
                        publishedAtUtc));
            }

            return feedItems;
        }
    }

