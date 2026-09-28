import { useMutation, useQueryClient } from '@tanstack/react-query';
import { createNewsSource } from '../api/newsSourcesApi';
import { newsSourcesQueryKey } from './useNewsSources';

export function useCreateNewsSource() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: createNewsSource,
        onSuccess: async () => {
            await queryClient.invalidateQueries({
                queryKey: newsSourcesQueryKey,
            });
        },
    });
}