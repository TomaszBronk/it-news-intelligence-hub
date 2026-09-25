import { ApiError } from './newsSourcesApi';

export async function getNewsItems() {
    const response = await fetch('/api/news-items?page=1&pageSize=50');

    if (!response.ok) {
        let message = 'Unable to load news items.';

        try {
            const error = await response.json();
            message = error.message ?? error.detail ?? error.title ?? message;
        } catch {
            // Keep default message.
        }

        throw new ApiError(message, response.status);
    }

    return response.json();
}