import { useState, useEffect } from "react";
import { PostDraftEditor } from "../components/PostDraftEditor";
import { generateSummary, getLatestSummary } from "../api/newsItemsApi";

const statuses = ['New', 'Read', 'Saved', 'Dismissed'];

export function NewsItemsList({
    items,
    updatingItemId,
    onUpdateStatus,
}) {
    const [summaries, setSummaries] = useState({}); // map newsItemId -> summaryText ("" means none)
    const [loadingSummary, setLoadingSummary] = useState({}); // map newsItemId -> boolean
    const [loadingGenerate, setLoadingGenerate] = useState({}); // kept for generate action UI

    useEffect(() => {
        let cancelled = false;

        async function loadExistingSummaries() {
            if (!items || items.length === 0) return;

            for (const item of items) {
                // skip if we already loaded this item's summary
                if (Object.prototype.hasOwnProperty.call(summaries, item.id)) {
                    continue;
                }

                setLoadingSummary(prev => ({ ...prev, [item.id]: true }));
                try {
                    const resp = await getLatestSummary(item.id);
                    if (cancelled) return;
                    setSummaries(prev => ({ ...prev, [item.id]: resp.content ?? "" }));
                } catch (e) {
                    // If not found or error, treat as no summary yet.
                    if (cancelled) return;
                    setSummaries(prev => ({ ...prev, [item.id]: "" }));
                } finally {
                    if (cancelled) return;
                    setLoadingSummary(prev => ({ ...prev, [item.id]: false }));
                }
            }
        }

        loadExistingSummaries();

        return () => {
            cancelled = true;
        };
        // Intentionally not adding `summaries` to deps to avoid refetch loops.
        // We only want to probe for existing summaries when `items` changes.
    }, [items]);

    async function handleGenerateSummary(id) {
        try {
            setLoadingGenerate(prev => ({ ...prev, [id]: true }));
            const response = await generateSummary(id);
            // response expected shape: NewsSummaryResponse { content }
            setSummaries(prev => ({ ...prev, [id]: response.content ?? "" }));
        } catch (e) {
            setSummaries(prev => ({ ...prev, [id]: "Błąd podczas generowania podsumowania." }));
        } finally {
            setLoadingGenerate(prev => ({ ...prev, [id]: false }));
        }
    }

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
                const summaryText = summaries[item.id] ?? null;
                const isLoadingExisting = !!loadingSummary[item.id];
                const isGenerating = !!loadingGenerate[item.id];

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

                        <div className="mb-2">
                            {isLoadingExisting ? (
                                <span className="text-sm text-gray-600">Sprawdzanie podsumowania...</span>
                            ) : summaryText && summaryText !== "" ? (
                                <div className="generated-summary border rounded p-3 mb-3 bg-gray-50">
                                    <strong>Istniejące podsumowanie:</strong>
                                    <div className="mt-2 whitespace-pre-wrap">
                                        {summaryText}
                                    </div>
                                </div>
                            ) : (
                                <button
                                    className="button button-primary button-small mr-2"
                                    type="button"
                                    onClick={() => handleGenerateSummary(item.id)}
                                    disabled={isGenerating}
                                >
                                    {isGenerating ? "Generowanie..." : "Wygeneruj podsumowanie"}
                                </button>
                            )}
                        </div>

                        <PostDraftEditor newsItemId={item.id} generated={summaryText ?? ""} />

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