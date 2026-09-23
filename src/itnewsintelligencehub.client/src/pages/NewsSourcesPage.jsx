import { useEffect, useState } from 'react';
import {
    ApiError,
    createNewsSource,
    getNewsSources,
} from '../api/newsSourcesApi';
import { NewsSourceForm } from '../components/NewsSourceForm';
import { NewsSourcesTable } from '../components/NewsSourcesTable';

export function NewsSourcesPage() {
    const [sources, setSources] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [isFormVisible, setIsFormVisible] = useState(false);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [errorMessage, setErrorMessage] = useState(null);

    useEffect(() => {
        void loadSources();
    }, []);

    async function loadSources() {
        setIsLoading(true);
        setErrorMessage(null);

        try {
            const response = await getNewsSources();
            setSources(response);
        } catch (error) {
            setErrorMessage(getErrorMessage(error));
        } finally {
            setIsLoading(false);
        }
    }

    async function handleCreateSource(request) {
        setIsSubmitting(true);
        setErrorMessage(null);

        try {
            const createdSource = await createNewsSource(request);

            setSources((current) =>
                [...current, createdSource].sort((left, right) =>
                    left.name.localeCompare(right.name),
                ),
            );

            setIsFormVisible(false);
        } catch (error) {
            setErrorMessage(getErrorMessage(error));
        } finally {
            setIsSubmitting(false);
        }
    }

    return (
        <main className="page-shell">
            <section className="page-header">
                <div>
                    <p className="eyebrow">IT News Intelligence Hub</p>
                    <h1>News sources</h1>
                    <p className="page-description">
                        Configure RSS and Atom feeds used to collect IT news for your
                        personal briefing.
                    </p>
                </div>

                {!isFormVisible && (
                    <button
                        className="button button-primary"
                        type="button"
                        onClick={() => setIsFormVisible(true)}
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
                            void loadSources();
                        }}
                    >
                        Try again
                    </button>
                </section>
            )}

            {isFormVisible && (
                <section className="content-card">
                    <NewsSourceForm
                        isSubmitting={isSubmitting}
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

                    <button
                        className="button button-secondary"
                        type="button"
                        onClick={() => {
                            void loadSources();
                        }}
                        disabled={isLoading}
                    >
                        {isLoading ? 'Loading...' : 'Refresh'}
                    </button>
                </div>

                {isLoading ? (
                    <div className="loading-state">Loading sources...</div>
                ) : (
                    <NewsSourcesTable sources={sources} />
                )}
            </section>
        </main>
    );
}

function getErrorMessage(error) {
    if (error instanceof ApiError) {
        return error.message;
    }

    if (error instanceof Error) {
        return error.message;
    }

    return 'An unexpected error occurred while communicating with the API.';
}