import { Navigate, createBrowserRouter } from 'react-router-dom';
import { AppLayout } from '../shared/components/AppLayout';

import { NewsItemsPage } from '../features/news-items/pages/NewsItemsPage';
import { NewsSourcesPage } from '../features/news-sources/pages/NewsSourcesPage';

import { SavedBriefingPage } from "../features/briefing/pages/SavedBriefingPage";
import { DailyBriefingPage } from "../features/briefing/pages/DailyBriefingPage";
import { WeeklyBriefingPage } from "../features/briefing/pages/WeeklyBriefingPage";

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
            {
                path: '/briefing/saved',
                element: <SavedBriefingPage />,
            },
            {
                path: '/briefing/daily',
                element: <DailyBriefingPage />,
            },
            {
                path: '/briefing/weekly',
                element: <WeeklyBriefingPage />,
            },
        ],
    },
]);