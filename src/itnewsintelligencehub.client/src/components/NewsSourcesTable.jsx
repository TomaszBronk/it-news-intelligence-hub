export function NewsSourcesTable({ sources }) {
    if (sources.length === 0) {
        return (
            <div className="empty-state">
                <h2>No news sources yet</h2>
                <p>
                    Add an RSS or Atom source to start building your IT news briefing.
                </p>
            </div>
        );
    }

    return (
        <div className="table-wrapper">
            <table>
                <thead>
                    <tr>
                        <th>Source</th>
                        <th>Category</th>
                        <th>Feed URL</th>
                        <th>Status</th>
                        <th>Last fetch</th>
                    </tr>
                </thead>

                <tbody>
                    {sources.map((source) => (
                        <tr key={source.id}>
                            <td>
                                <div className="source-name">
                                    <strong>{source.name}</strong>

                                    {source.websiteUrl && (
                                        <a
                                            href={source.websiteUrl}
                                            target="_blank"
                                            rel="noreferrer"
                                        >
                                            Website
                                        </a>
                                    )}
                                </div>
                            </td>

                            <td>
                                <span className="category-badge">{source.category}</span>
                            </td>

                            <td className="feed-url">
                                <a
                                    href={source.feedUrl}
                                    target="_blank"
                                    rel="noreferrer"
                                    title={source.feedUrl}
                                >
                                    {source.feedUrl}
                                </a>
                            </td>

                            <td>
                                <span
                                    className={
                                        source.isActive
                                            ? 'status-badge status-active'
                                            : 'status-badge status-inactive'
                                    }
                                >
                                    {source.isActive ? 'Active' : 'Inactive'}
                                </span>
                            </td>

                            <td>{formatDate(source.lastFetchedAtUtc)}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

function formatDate(value) {
    if (!value) {
        return 'Not fetched yet';
    }

    return new Intl.DateTimeFormat('en-GB', {
        dateStyle: 'medium',
        timeStyle: 'short',
    }).format(new Date(value));
}