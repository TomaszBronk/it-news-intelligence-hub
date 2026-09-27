import { useQuery } from '@tanstack/react-query';
import { getNewsItems } from '../api/newsItemsApi';

const newsItemsRefreshIntervalMs = 60_000;

export function useNewsItems(filters = {}) {
    return useQuery({
        queryKey: ['news-items', filters],
        queryFn: () => getNewsItems(filters),
        refetchInterval: newsItemsRefreshIntervalMs,
        refetchOnWindowFocus: true,
    });
}