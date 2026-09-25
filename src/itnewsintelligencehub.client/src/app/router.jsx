import { Navigate, createBrowserRouter } from 'react-router-dom';
import { AppLayout } from '../shared/components/AppLayout';
import { NewsItemsPage } from '../features/news-items/pages/NewsItemsPage';
import { NewsSourcesPage } from '../features/news-sources/pages/NewsSourcesPage';

export const router = createBrowserRouter([
    {
        element: <AppLayout />,
        children: [
            {
                path: '/',
                element: <Navigate to="/sources" replace />,
            },
            {
                path: '/sources',
                element: <NewsSourcesPage />,
            },
            {
                path: '/news',
                element: <NewsItemsPage />,
            },
        ],
    },
]);