import { useMutation, useQueryClient } from '@tanstack/react-query';
import { updateNewsItemStatus } from '../api/newsItemsApi';

export function useUpdateNewsItemStatus() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: updateNewsItemStatus,
        onSuccess: async () => {
            await queryClient.invalidateQueries({
                queryKey: ['news-items'],
            });
        },
    });
}