import { useQuery } from '@tanstack/react-query';
import { getNewsItems } from '../api/newsItemsApi';

export function useNewsItems(filters = {}) {
    return useQuery({
        queryKey: ['news-items', filters],
        queryFn: () => getNewsItems(filters),
    });
}