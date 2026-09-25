import { apiClient } from '../../../shared/api/apiClient';

export async function getNewsSources() {
    const response = await apiClient.get('/news-sources');

    return response.data;
}

export async function createNewsSource(request) {
    const response = await apiClient.post('/news-sources', request);

    return response.data;
}

export async function fetchNewsSource(sourceId) {
    const response = await apiClient.post(`/news-sources/${sourceId}/fetch`);

    return response.data;
}