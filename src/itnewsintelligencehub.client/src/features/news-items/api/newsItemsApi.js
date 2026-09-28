import { apiClient } from '../../../shared/api/apiClient';

export async function getNewsItems({
    sourceId,
    category,
    status,
    publishedFromUtc,
    publishedToUtc,
    search,
    page = 1,
    pageSize = 50,
} = {}) {
    const response = await apiClient.get('/news-items', {
        params: {
            sourceId: sourceId || undefined,
            category: category || undefined,
            status: status || undefined,
            publishedFromUtc: publishedFromUtc || undefined,
            publishedToUtc: publishedToUtc || undefined,
            search: search || undefined,
            page,
            pageSize,
        },
    });

    return response.data;
}

export async function updateNewsItemStatus({ id, status }) {
    const response = await apiClient.patch(`/news-items/${id}/status`, {
        status,
    });

    return response.data;
}