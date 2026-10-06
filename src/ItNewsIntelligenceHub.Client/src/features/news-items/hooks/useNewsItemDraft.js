import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
    generatePostDraft,
    generateDiscussionPrompt,
    getLatestDraft,
    updateDraft,
} from "../api/newsItemsApi";


export function useLatestDraft(newsItemId) {
    return useQuery({
        queryKey: ["news-draft", newsItemId],
        queryFn: () => getLatestDraft(newsItemId),
        enabled: !!newsItemId,
    });
}


export function useGeneratePostDraft(newsItemId) {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: () => generatePostDraft(newsItemId),
        onSuccess: (data) => {
            queryClient.setQueryData(
                ["news-draft", newsItemId],
                data
            );
        },
    });
}


export function useGenerateDiscussionPrompt(newsItemId) {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: () => generateDiscussionPrompt(newsItemId),
        onSuccess: (data) => {
            queryClient.setQueryData(
                ["news-draft", newsItemId],
                data
            );
        },
    });
}


export function useUpdateDraft(draftId, newsItemId) {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: (updates) => updateDraft(newsItemId, draftId, updates),
        onSuccess: (data) => {
            queryClient.setQueryData(
                ["news-draft", newsItemId],
                data
            );
        },
    });
}