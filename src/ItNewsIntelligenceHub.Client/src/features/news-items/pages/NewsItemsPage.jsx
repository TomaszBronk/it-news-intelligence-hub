import { useState } from 'react';
import { NewsItemsList } from '../components/NewsItemsList';
import { NewsItemsFilters } from '../components/NewsItemsFilters';
import { createEmptyFilters } from '../model/newsItemsFilters'
import { useNewsItems } from '../hooks/useNewsItems';
import { useUpdateNewsItemStatus } from '../hooks/useUpdateNewsItemStatus';
import { useNewsSources } from '../../news-sources/hooks/useNewsSources';


export function NewsItemsPage() {
    const [filters, setFilters] = useState(createEmptyFilters());

    const {
        data: items = [],
        isPending: isItemsPending,
        isFetching: isItemsFetching,
        error: itemsError,
        refetch: refetchItems,
    } = useNewsItems(filters);

    const {
        data: sources = [],
        isPending: isSourcesPending,
    } = useNewsSources();

    const updateStatusMutation = useUpdateNewsItemStatus();

    async function handleUpdateStatus(id, status) {
        try {
            await updateStatusMutation.mutateAsync({ id, status });
        } catch {
            // The error is rendered below from mutation state.
        }
    }

    const errorMessage =
        getErrorMessage(updateStatusMutation.error)
        ?? getErrorMessage(itemsError);

    const isFiltersLoading = isItemsFetching || isSourcesPending;

    return (
        <main className="page-shell">
            <section className="page-header">
                <div>
                    <p className="eyebrow">IT News Intelligence Hub</p>
                    <h1>News items</h1>
                    <p className="page-description">
                        Browse imported IT news, filter the list and manage saved items.
                        The list refreshes automatically every minute.
                    </p>
                </div>

                <button
                    className="button button-secondary"
                    type="button"
                    onClick={() => {
                        void refetchItems();
                    }}
                    disabled={isItemsFetching}
                >
                    {isItemsFetching ? 'Refreshing...' : 'Refresh'}
                </button>
            </section>

            {errorMessage && (
                <section className="alert alert-error" role="alert">
                    <div>
                        <strong>Request failed.</strong>
                        <p>{errorMessage}</p>
                    </div>
                </section>
            )}

            <section className="content-card">
                <div className="section-heading">
                    <div>
                        <h2>Filters</h2>
                        <p>Use filters to focus on the news that matters to you.</p>
                    </div>
                </div>

                <NewsItemsFilters
                    sources={sources}
                    initialFilters={filters}
                    onApply={(newFilters) => setFilters(newFilters)}
                    onReset={(emptyFilters) => setFilters(emptyFilters)}
                    isLoading={isFiltersLoading}
                />
            </section>

            <section className="content-card">
                <div className="section-heading">
                    <div>
                        <h2>Imported news</h2>
                        <p>
                            {items.length} {items.length === 1 ? 'item' : 'items'}
                        </p>
                    </div>

                    <p className="refresh-status">
                        {isItemsFetching
                            ? 'Refreshing data...'
                            : 'Auto-refresh: every 60 seconds'}
                    </p>
                </div>

                {isItemsPending ? (
                    <div className="loading-state">
                        Loading imported news...
                    </div>
                ) : (
                    <NewsItemsList
                        items={items}
                        updatingItemId={
                            updateStatusMutation.isPending
                                ? updateStatusMutation.variables?.id
                                : null
                        }
                        onUpdateStatus={handleUpdateStatus}
                    />
                )}
            </section>
        </main>
    );
}

function getErrorMessage(error) {
    if (!error) {
        return null;
    }

    if (error instanceof Error) {
        return error.message;
    }

    return 'An unexpected error occurred while communicating with the API.';
}