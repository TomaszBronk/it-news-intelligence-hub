import { useEffect, useState } from 'react';
import { getNewsItems } from '../api/newsItemsApi';
import { NewsItemsList } from '../components/NewsItemsList';

export function NewsItemsPage() {
    const [items, setItems] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [errorMessage, setErrorMessage] = useState(null);

    useEffect(() => {
        void loadItems();
    }, []);

    async function loadItems() {
        setIsLoading(true);
        setErrorMessage(null);

        try {
            const response = await getNewsItems();
            setItems(response);
        } catch (error) {
            setErrorMessage(
                error instanceof Error
                    ? error.message
                    : 'Unable to load news items.',
            );
        } finally {
            setIsLoading(false);
        }
    }

    return (
        <main className="page-shell">
            <section className="page-header">
                <div>
                    <p className="eyebrow">IT News Intelligence Hub</p>
                    <h1>Imported news</h1>
                    <p className="page-description">
                        News collected from configured RSS and Atom sources.
                    </p>
                </div>

                <button
                    className="button button-secondary"
                    type="button"
                    onClick={() => {
                        void loadItems();
                    }}
                    disabled={isLoading}
                >
                    {isLoading ? 'Loading...' : 'Refresh'}
                </button>
            </section>

            {errorMessage && (
                <section className="alert alert-error" role="alert">
                    <div>
                        <strong>Unable to load news.</strong>
                        <p>{errorMessage}</p>
                    </div>
                </section>
            )}

            <section className="content-card">
                {isLoading ? (
                    <div className="loading-state">Loading imported news...</div>
                ) : (
                    <NewsItemsList items={items} />
                )}
            </section>
        </main>
    );
}