export function NewsSourcesTable({
    sources,
    fetchingSourceId,
    onFetch,
}) {
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
                        <th>Status</th>
                        <th>Last attempt</th>
                        <th>Last successful import</th>
                        <th>Actions</th>
                    </tr>
                </thead>

                <tbody>
                    {sources.map((source) => {
                        const isFetching = fetchingSourceId === source.id;
                        const status = getImportStatus(source);

                        return (
                            <tr key={source.id}>
                                <td>
                                    <div className="source-name">
                                        <strong>{source.name}</strong>

                                        <a
                                            href={source.feedUrl}
                                            target="_blank"
                                            rel="noreferrer"
                                            title={source.feedUrl}
                                        >
                                            RSS / Atom feed
                                        </a>

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
                                    <span className="category-badge">
                                        {source.category}
                                    </span>
                                </td>

                                <td>
                                    <ImportStatusBadge status={status} />
                                </td>

                                <td>{formatDate(source.lastFetchAttemptAtUtc)}</td>

                                <td>{formatDate(source.lastSuccessfulFetchAtUtc)}</td>

                                <td>
                                    <div className="source-actions">
                                        <button
                                            className="button button-secondary button-small"
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
                                            <span className="inactive-note">Inactive</span>
                                        )}
                                    </div>

                                    {source.lastFetchError && (
                                        <p className="import-error-message" role="alert">
                                            {source.lastFetchError}
                                        </p>
                                    )}
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