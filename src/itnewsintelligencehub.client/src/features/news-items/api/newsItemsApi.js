import { apiClient } from '../../../shared/api/apiClient';

export async function getNewsItems({
    sourceId,
    category,
    page = 1,
    pageSize = 50,
} = {}) {
    const response = await apiClient.get('/news-items', {
        params: {
            sourceId: sourceId || undefined,
            category: category || undefined,
            page,
            pageSize,
        },
    });

    return response.data;
}