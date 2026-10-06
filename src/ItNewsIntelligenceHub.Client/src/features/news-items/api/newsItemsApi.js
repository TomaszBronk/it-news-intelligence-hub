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

export async function generatePostDraft(newsItemId) {
    const response = await apiClient.post(
        `/news-items/${newsItemId}/draft/generate-post`
    );
    return response.data;
}

export async function generateDiscussionPrompt(newsItemId) {
    const response = await apiClient.post(
        `/news-items/${newsItemId}/draft/generate-discussion`
    );
    return response.data;
}

export async function getLatestDraft(newsItemId) {
    const response = await apiClient.get(
        `/news-items/${newsItemId}/draft`
    );
    return response.data;
}

export async function updateDraft(newsItemId,draftId, updates) {
    const response = await apiClient.patch(
        `/news-items/${newsItemId}/draft/${draftId}`,
        updates
    );
    return response.data;
}
export async function generateSummary(newsItemId) {
    const response = await apiClient.post(
        `/news-items/${newsItemId}/summary`,
    );
    return response.data;
}

export async function getLatestSummary(newsItemId) {
    const response = await apiClient.get(
        `/news-items/${newsItemId}/summary`,
    );
    return response.data;
}