import { apiClient } from '../../../shared/api/apiClient';

export async function getBriefing(type) {
    const response = await apiClient.get(`/briefings/${type}`);
    return response.data;
}