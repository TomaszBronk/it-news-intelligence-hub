import { useMutation, useQueryClient } from '@tanstack/react-query';
import { fetchNewsSource } from '../api/newsSourcesApi';
import { newsSourcesQueryKey } from './useNewsSources';

export function useFetchNewsSource() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: fetchNewsSource,
        onSuccess: async () => {
            await Promise.all([
                queryClient.invalidateQueries({
                    queryKey: newsSourcesQueryKey,
                }),
                queryClient.invalidateQueries({
                    queryKey: ['news-items'],
                }),
            ]);
        },
    });
}