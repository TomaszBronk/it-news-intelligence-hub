import { useQuery } from '@tanstack/react-query';
import { getNewsSources } from '../api/newsSourcesApi';

export const newsSourcesQueryKey = ['news-sources'];

export function useNewsSources() {
    return useQuery({
        queryKey: newsSourcesQueryKey,
        queryFn: getNewsSources,
    });
}