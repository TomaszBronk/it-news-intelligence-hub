import { useQuery } from '@tanstack/react-query';
import { getNewsSources } from '../api/newsSourcesApi';

export const newsSourcesQueryKey = ['news-sources'];

const sourcesRefreshIntervalMs = 30_000;

export function useNewsSources() {
    return useQuery({
        queryKey: newsSourcesQueryKey,
        queryFn: getNewsSources,
        refetchInterval: sourcesRefreshIntervalMs,
        refetchOnWindowFocus: true,
    });
}