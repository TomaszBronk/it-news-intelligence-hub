import { useState } from 'react';
import { createEmptyFilters, normalizeFilters } from '../model/newsItemsFilters';

const categories = [
    'DotNet',
    'Azure',
    'AI',
    'Security',
    'React',
    'DevOps',
    'Data',
    'Other',
];

const statuses = [
    'New',
    'Read',
    'Saved',
    'Dismissed',
];

export function NewsItemsFilters({
    sources,
    initialFilters,
    onApply,
    onReset,
    isLoading,
}) {
    const [filters, setFilters] = useState(() => ({
        ...initialFilters,
    }));

    function handleSubmit(event) {
        event.preventDefault();

        onApply(normalizeFilters(filters));
    }

    function handleReset() {
        const emptyFilters = createEmptyFilters();

        setFilters(emptyFilters);
        onReset(emptyFilters);
    }

    return (
        <form className="news-filters" onSubmit={handleSubmit}>
            <div className="filters-grid">
                <label>
                    <span>Search title</span>
                    <input
                        type="search"
                        value={filters.search}
                        placeholder="e.g. Azure, .NET, React"
                        onChange={(event) =>
                            setFilters((current) => ({
                                ...current,
                                search: event.target.value,
                            }))
                        }
                        disabled={isLoading}
                    />
                </label>

                <label>
                    <span>Status</span>
                    <select
                        value={filters.status}
                        onChange={(event) =>
                            setFilters((current) => ({
                                ...current,
                                status: event.target.value,
                            }))
                        }
                        disabled={isLoading}
                    >
                        <option value="">All statuses</option>

                        {statuses.map((status) => (
                            <option key={status} value={status}>
                                {status}
                            </option>
                        ))}
                    </select>
                </label>

                <label>
                    <span>Category</span>
                    <select
                        value={filters.category}
                        onChange={(event) =>
                            setFilters((current) => ({
                                ...current,
                                category: event.target.value,
                            }))
                        }
                        disabled={isLoading}
                    >
                        <option value="">All categories</option>

                        {categories.map((category) => (
                            <option key={category} value={category}>
                                {category}
                            </option>
                        ))}
                    </select>
                </label>

                <label>
                    <span>Source</span>
                    <select
                        value={filters.sourceId}
                        onChange={(event) =>
                            setFilters((current) => ({
                                ...current,
                                sourceId: event.target.value,
                            }))
                        }
                        disabled={isLoading}
                    >
                        <option value="">All sources</option>

                        {sources.map((source) => (
                            <option key={source.id} value={source.id}>
                                {source.name}
                            </option>
                        ))}
                    </select>
                </label>

                <label>
                    <span>Published from</span>
                    <input
                        type="date"
                        value={filters.publishedFromUtc}
                        onChange={(event) =>
                            setFilters((current) => ({
                                ...current,
                                publishedFromUtc: event.target.value,
                            }))
                        }
                        disabled={isLoading}
                    />
                </label>

                <label>
                    <span>Published to</span>
                    <input
                        type="date"
                        value={filters.publishedToUtc}
                        onChange={(event) =>
                            setFilters((current) => ({
                                ...current,
                                publishedToUtc: event.target.value,
                            }))
                        }
                        disabled={isLoading}
                    />
                </label>
            </div>

            <div className="filter-actions">
                <button
                    className="button button-primary"
                    type="submit"
                    disabled={isLoading}
                >
                    Apply filters
                </button>

                <button
                    className="button button-secondary"
                    type="button"
                    onClick={handleReset}
                    disabled={isLoading}
                >
                    Reset
                </button>
            </div>
        </form>
    );
}