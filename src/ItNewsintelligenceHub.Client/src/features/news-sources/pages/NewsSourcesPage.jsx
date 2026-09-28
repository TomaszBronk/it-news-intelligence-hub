import { useState } from 'react';
import { NewsSourceForm } from '../components/NewsSourceForm';
import { NewsSourcesTable } from '../components/NewsSourcesTable';
import { useCreateNewsSource } from '../hooks/useCreateNewsSource';
import { useFetchNewsSource } from '../hooks/useFetchNewsSource';
import { useNewsSources } from '../hooks/useNewsSources';

export function NewsSourcesPage() {
    const [isFormVisible, setIsFormVisible] = useState(false);
    const [successMessage, setSuccessMessage] = useState(null);

    const {
        data: sources = [],
        isPending,
        isFetching,
        error,
        refetch,
    } = useNewsSources();

    const createSourceMutation = useCreateNewsSource();
    const fetchSourceMutation = useFetchNewsSource();

    async function handleCreateSource(request) {
        setSuccessMessage(null);

        try {
            await createSourceMutation.mutateAsync(request);

            setIsFormVisible(false);
            setSuccessMessage('News source was added successfully.');
        } catch {
            // Error is rendered below from mutation state.
        }
    }

    async function handleFetchSource(sourceId) {
        setSuccessMessage(null);

        try {
            const result = await fetchSourceMutation.mutateAsync(sourceId);

            setSuccessMessage(
                `Import completed: ${result.importedItemsCount} new item(s) imported, `
                + `${result.skippedItemsCount} existing item(s) skipped.`,
            );
        } catch {
            // Error is rendered below from mutation state.
        }
    }

    const errorMessage =
        getErrorMessage(createSourceMutation.error)
        ?? getErrorMessage(fetchSourceMutation.error)
        ?? getErrorMessage(error);

    return (
        <main className="page-shell">
            <section className="page-header">
                <div>
                    <p className="eyebrow">IT News Intelligence Hub</p>
                    <h1>News sources</h1>
                    <p className="page-description">
                        Configure RSS and Atom feeds used to collect IT news for your
                        personal briefing. Import status refreshes automatically every 30 seconds.
                    </p>
                </div>

                {!isFormVisible && (
                    <button
                        className="button button-primary"
                        type="button"
                        onClick={() => {
                            setIsFormVisible(true);
                            setSuccessMessage(null);
                        }}
                    >
                        Add source
                    </button>
                )}
            </section>

            {errorMessage && (
                <section className="alert alert-error" role="alert">
                    <div>
                        <strong>Request failed.</strong>
                        <p>{errorMessage}</p>
                    </div>

                    <button
                        className="button button-secondary"
                        type="button"
                        onClick={() => {
                            void refetch();
                        }}
                        disabled={isFetching}
                    >
                        {isFetching ? 'Refreshing...' : 'Refresh'}
                    </button>
                </section>
            )}

            {successMessage && (
                <section className="alert alert-success" role="status">
                    <div>
                        <strong>Success.</strong>
                        <p>{successMessage}</p>
                    </div>
                </section>
            )}

            {isFormVisible && (
                <section className="content-card">
                    <NewsSourceForm
                        isSubmitting={createSourceMutation.isPending}
                        onSubmit={handleCreateSource}
                        onCancel={() => setIsFormVisible(false)}
                    />
                </section>
            )}

            <section className="content-card">
                <div className="section-heading">
                    <div>
                        <h2>Configured sources</h2>
                        <p>
                            {sources.length} {sources.length === 1 ? 'source' : 'sources'}
                        </p>
                    </div>

                    <p className="refresh-status">
                        {isFetching ? 'Refreshing data...' : 'Auto-refresh: every 30 seconds'}
                    </p>

                    <button
                        className="button button-secondary"
                        type="button"
                        onClick={() => {
                            void refetch();
                        }}
                        disabled={isPending}
                    >
                        {isPending ? 'Loading...' : 'Refresh'}
                    </button>
                </div>

                {isPending ? (
                    <div className="loading-state">Loading sources...</div>
                ) : (
                    <NewsSourcesTable
                        sources={sources}
                        fetchingSourceId={
                            fetchSourceMutation.isPending
                                ? fetchSourceMutation.variables
                                : null
                        }
                        onFetch={handleFetchSource}
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