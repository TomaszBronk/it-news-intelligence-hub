export function NewsItemsList({ items }) {
    if (items.length === 0) {
        return (
            <div className="empty-state">
                <h2>No imported news yet</h2>
                <p>
                    Go to News sources and use “Fetch now” to import the first RSS or
                    Atom feed.
                </p>
            </div>
        );
    }

    return (
        <div className="news-items-list">
            {items.map((item) => (
                <article className="news-item-card" key={item.id}>
                    <div className="news-item-meta">
                        <span className="category-badge">{item.category}</span>
                        <span>{item.sourceName}</span>
                        <span>{formatDate(item.publishedAtUtc ?? item.retrievedAtUtc)}</span>
                    </div>

                    <h2>{item.title}</h2>

                    {item.summary && <p>{stripHtml(item.summary)}</p>}

                    <div className="news-item-footer">
                        {item.author && <span>By {item.author}</span>}

                        <a
                            href={item.originalUrl}
                            target="_blank"
                            rel="noreferrer"
                        >
                            Open original source →
                        </a>
                    </div>
                </article>
            ))}
        </div>
    );
}

function formatDate(value) {
    return new Intl.DateTimeFormat('en-GB', {
        dateStyle: 'medium',
        timeStyle: 'short',
    }).format(new Date(value));
}

function stripHtml(value) {
    const element = document.createElement('div');
    element.innerHTML = value;

    return element.textContent || element.innerText || '';
}