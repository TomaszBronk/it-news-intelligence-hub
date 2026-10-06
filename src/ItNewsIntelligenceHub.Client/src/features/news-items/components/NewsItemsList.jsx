import { useState, useEffect } from "react";
import { PostDraftEditor } from "../components/PostDraftEditor";
import { generateSummary, getLatestSummary } from "../api/newsItemsApi";

const statuses = ['New', 'Read', 'Saved', 'Dismissed'];

export function NewsItemsList({
    items,
    updatingItemId,
    onUpdateStatus,
}) {
    const [summaries, setSummaries] = useState({}); // newsItemId -> summaryText ("" means none)
    const [loadingSummary, setLoadingSummary] = useState({}); // newsItemId -> boolean
    const [loadingGenerate, setLoadingGenerate] = useState({}); // newsItemId -> boolean

    useEffect(() => {
        let cancelled = false;

        async function loadExistingSummaries() {
            if (!items || items.length === 0) return;

            for (const item of items) {
                if (Object.prototype.hasOwnProperty.call(summaries, item.id)) {
                    continue;
                }

                setLoadingSummary(prev => ({ ...prev, [item.id]: true }));
                try {
                    const resp = await getLatestSummary(item.id);
                    if (cancelled) return;
                    setSummaries(prev => ({ ...prev, [item.id]: resp.content ?? "" }));
                } catch (e) {
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
    }, [items]);

    async function handleGenerateSummary(id) {
        try {
            setLoadingGenerate(prev => ({ ...prev, [id]: true }));
            const response = await generateSummary(id);
            setSummaries(prev => ({ ...prev, [id]: response.content ?? "" }));
        } catch (e) {
            setSummaries(prev => ({ ...prev, [id]: "Error generating summary." }));
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
                        <div className="news-item-header">
                            <div className="news-item-meta">
                                <span className="category-badge">{item.category}</span>
                                <StatusBadge status={item.status} />
                                <span className="news-source-name">{item.sourceName}</span>
                                <span className="news-published-date">
                                    {formatDate(item.publishedAtUtc ?? item.retrievedAtUtc)}
                                </span>
                            </div>
                        </div>

                        <div className="news-item-content">
                            <h2 className="news-item-title">{item.title}</h2>

                            {item.summary && (
                                <p className="news-original-summary">
                                    {stripHtml(item.summary)}
                                </p>
                            )}
                        </div>

                        <section className="ai-generated-section">
                            <div className="ai-section-header">
                                <div>
                                    <span className="ai-section-label">AI-generated content</span>
                                    <p className="ai-section-description">
                                        Review and edit before sharing.
                                    </p>
                                </div>

                                <span className="ai-section-badge">AI</span>
                            </div>

                            <div className="ai-section-content">
                                <div className="ai-content-block">
                                    <div className="ai-content-heading">
                                        <h3>Summary</h3>
                                        <span>Polish summary</span>
                                    </div>

                                    {isLoadingExisting ? (
                                        <p className="ai-loading-text">
                                            Checking for existing summary...
                                        </p>
                                    ) : summaryText && summaryText !== "" ? (
                                        <div className="ai-text-box">
                                            {summaryText}
                                        </div>
                                    ) : (
                                        <button
                                            className="news-action-button button-primary"
                                            type="button"
                                            onClick={() => handleGenerateSummary(item.id)}
                                            disabled={isGenerating}
                                        >
                                            {isGenerating
                                                ? "Generating..."
                                                : "Generate summary"}
                                        </button>
                                    )}
                                </div>

                                <div className="ai-content-block">
                                    <div className="ai-content-heading">
                                        <h3>Post & discussion</h3>
                                        <span>Editable draft</span>
                                    </div>

                                    <PostDraftEditor
                                        newsItemId={item.id}
                                        generated={summaryText ?? ""}
                                    />
                                </div>
                            </div>
                        </section>

                        <div className="news-item-footer">
                            <div className="news-item-author">
                                {item.author && <span>By {item.author}</span>}
                            </div>

                            <a
                                href={item.originalUrl}
                                target="_blank"
                                rel="noreferrer"
                                className="original-source-link"
                            >
                                Open original source
                                <span aria-hidden="true"> →</span>
                            </a>
                        </div>

                        <div className="news-item-actions">
                            <button
                                className="news-action-button button-secondary"
                                type="button"
                                onClick={() => onUpdateStatus(item.id, "Read")}
                                disabled={isUpdating || item.status === "Read"}
                            >
                                Mark as read
                            </button>

                            <button
                                className="news-action-button button-secondary"
                                type="button"
                                onClick={() => onUpdateStatus(item.id, "Saved")}
                                disabled={isUpdating || item.status === "Saved"}
                            >
                                Save
                            </button>

                            <button
                                className="news-action-button button-secondary"
                                type="button"
                                onClick={() => onUpdateStatus(item.id, "Dismissed")}
                                disabled={isUpdating || item.status === "Dismissed"}
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