import { useState } from 'react';
import { NewsItemsPage } from './pages/NewsItemsPage';
import { NewsSourcesPage } from './pages/NewsSourcesPage';
import './index.css';

function App() {
    const [page, setPage] = useState('sources');

    return (
        <>
            <header className="app-navigation">
                <div className="app-navigation-content">
                    <button
                        className="app-brand"
                        type="button"
                        onClick={() => setPage('sources')}
                    >
                        IT News Intelligence Hub
                    </button>

                    <nav className="app-navigation-links" aria-label="Main navigation">
                        <button
                            className={page === 'sources' ? 'navigation-link active' : 'navigation-link'}
                            type="button"
                            onClick={() => setPage('sources')}
                        >
                            Sources
                        </button>

                        <button
                            className={page === 'news' ? 'navigation-link active' : 'navigation-link'}
                            type="button"
                            onClick={() => setPage('news')}
                        >
                            News
                        </button>
                    </nav>
                </div>
            </header>

            {page === 'sources' ? <NewsSourcesPage /> : <NewsItemsPage />}
        </>
    );
}

export default App;