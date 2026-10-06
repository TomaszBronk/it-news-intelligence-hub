export function NewsSourcesTable({
    sources,
    fetchingSourceId,
    onFetch,
}) {
    if (sources.length === 0) {
        return (
            <div className="empty-state sources-empty-state">
                <div className="sources-empty-icon" aria-hidden="true">
                    RSS
                </div>

                <h2>No news sources yet</h2>

                <p>
                    Add an RSS or Atom source to start building your IT news
                    briefing.
                </p>
            </div>
        );
    }

    return (
        <div className="table-wrapper sources-table-wrapper">
            <table className="sources-table">
                <thead>
                    <tr>
                        <th>Source</th>
                        <th>Category</th>
                        <th>Status</th>
                        <th>Last attempt</th>
                        <th>Last successful import</th>
                        <th >Actions</th>
                    </tr>
                </thead>

                <tbody>
                    {sources.map((source) => {
                        const isFetching = fetchingSourceId === source.id;
                        const status = getImportStatus(source);

                        return (
                            <tr key={source.id}>
                                <td className="source-main-cell">
                                    <div className="source-name">
                                        <div className="source-name-row">
                                            <strong>{source.name}</strong>

                                            <span
                                                className={
                                                    source.isActive
                                                        ? 'source-active-badge'
                                                        : 'source-inactive-badge'
                                                }
                                            >
                                                {source.isActive
                                                    ? 'Active'
                                                    : 'Inactive'}
                                            </span>
                                        </div>

                                        <div className="source-links">
                                            <a
                                                className="source-link"
                                                href={source.feedUrl}
                                                target="_blank"
                                                rel="noreferrer"
                                                title={source.feedUrl}
                                            >
                                                RSS / Atom feed
                                            </a>

                                            {source.websiteUrl && (
                                                <>
                                                    <span
                                                        className="source-link-separator"
                                                        aria-hidden="true"
                                                    >
                                                        ·
                                                    </span>

                                                    <a
                                                        className="source-link"
                                                        href={source.websiteUrl}
                                                        target="_blank"
                                                        rel="noreferrer"
                                                    >
                                                        Website
                                                    </a>
                                                </>
                                            )}
                                        </div>

                                        <span
                                            className="source-feed-url"
                                            title={source.feedUrl}
                                        >
                                            {source.feedUrl}
                                        </span>
                                    </div>
                                </td>

                                <td>
                                    <span className="category-badge source-category-badge">
                                        {source.category}
                                    </span>
                                </td>

                                <td>
                                    <ImportStatusBadge status={status} />
                                </td>

                                <td className="source-date-cell">
                                    {formatDate(source.lastFetchAttemptAtUtc)}
                                </td>

                                <td className="source-date-cell">
                                    {formatDate(source.lastSuccessfulFetchAtUtc)}
                                </td>

                                <td className="sources-actions-cell">
                                    <div className="source-actions">
                                        <button
                                            className="button button-secondary button-small source-fetch-button"
                                            type="button"
                                            onClick={() => onFetch(source.id)}
                                            disabled={!source.isActive || isFetching}
                                            title={
                                                source.isActive
                                                    ? 'Import this feed now'
                                                    : 'This source is inactive'
                                            }
                                        >
                                            {isFetching ? 'Fetching...' : 'Fetch now'}
                                        </button>

                                        {!source.isActive && (
                                            <span className="inactive-note">
                                                Inactive source
                                            </span>
                                        )}
                   
                                    </div>
                      
                      
                                </td>
                            
                     
                                 
                            </tr>
                        );
                    })}
                </tbody>
            </table>
        </div>
    );
}

function ImportStatusBadge({ status }) {
    return (
        <span className={`import-status-badge import-status-${status.kind}`}>
            {status.label}
        </span>
    );
}

function getImportStatus(source) {
    if (source.lastFetchError) {
        return {
            kind: 'failed',
            label: 'Failed',
        };
    }

    if (source.lastSuccessfulFetchAtUtc) {
        return {
            kind: 'success',
            label: 'Success',
        };
    }

    return {
        kind: 'pending',
        label: 'Not imported yet',
    };
}

function formatDate(value) {
    if (!value) {
        return '—';
    }

    return new Intl.DateTimeFormat('en-GB', {
        dateStyle: 'medium',
        timeStyle: 'short',
    }).format(new Date(value));
}