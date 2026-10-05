const statuses = ['New', 'Read', 'Saved', 'Dismissed'];

export function NewsItemsList({
    items,
    updatingItemId,
    onUpdateStatus,
}) {
    if (items.length === 0) {
        return (
            <div className="empty-state">
                <h2>No imported news found</h2>
                <p>
                    Try changing the filters or import news from an active RSS or Atom
                    source.
                </p>
            </div>
        );
    }

    return (
        <div className="news-items-list">
            {items.map((item) => {
                const isUpdating = updatingItemId === item.id;

                return (
                    <article className="news-item-card" key={item.id}>
                        <div className="news-item-meta">
                            <span className="category-badge">{item.category}</span>
                            <StatusBadge status={item.status} />
                            <span>{item.sourceName}</span>
                            <span>
                                {formatDate(item.publishedAtUtc ?? item.retrievedAtUtc)}
                            </span>
                        </div>

                        <h2>{item.title}</h2>

                        {item.summary && (
                            <p>{stripHtml(item.summary)}</p>
                        )}

                        <div className="news-item-footer">
                            <div className="news-item-author">
                                {item.author && <span>By {item.author}</span>}
                            </div>

                            <a
                                href={item.originalUrl}
                                target="_blank"
                                rel="noreferrer"
                            >
                                Open original source →
                            </a>
                        </div>

                        <div className="news-item-actions">
                            <button
                                className="button button-secondary button-small"
                                type="button"
                                onClick={() => onUpdateStatus(item.id, 'Read')}
                                disabled={isUpdating || item.status === 'Read'}
                            >
                                Mark as read
                            </button>

                            <button
                                className="button button-secondary button-small"
                                type="button"
                                onClick={() => onUpdateStatus(item.id, 'Saved')}
                                disabled={isUpdating || item.status === 'Saved'}
                            >
                                Save
                            </button>

                            <button
                                className="button button-secondary button-small"
                                type="button"
                                onClick={() => onUpdateStatus(item.id, 'Dismissed')}
                                disabled={isUpdating || item.status === 'Dismissed'}
                            >
                                Dismiss
                            </button>

                            <label className="status-select">
                                <span>Change status</span>

                                <select
                                    value={item.status}
                                    onChange={(event) =>
                                        onUpdateStatus(item.id, event.target.value)
                                    }
                                    disabled={isUpdating}
                                >
                                    {statuses.map((status) => (
                                        <option key={status} value={status}>
                                            {status}
                                        </option>
                                    ))}
                                </select>
                            </label>

                            {isUpdating && (
                                <span className="updating-status">
                                    Updating...
                                </span>
                            )}
                        </div>
                    </article>
                );
            })}
        </div>
    );
}

function StatusBadge({ status }) {
    const normalizedStatus =
        typeof status === 'string'
            ? status.toLowerCase()
            : 'unknown';

    const label =
        typeof status === 'string'
            ? status
            : 'Unknown';

    return (
        <span className={`news-status-badge news-status-${normalizedStatus}`}>
            {label}
        </span>
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