import { useState, useEffect } from "react";
import {
    useLatestDraft,
    useGeneratePostDraft,
    useGenerateDiscussionPrompt,
    useUpdateDraft,
} from "../hooks/useNewsItemDraft";


export function PostDraftEditor({ newsItemId }) {
    const { data: draft, isLoading, error } = useLatestDraft(newsItemId);

    const { mutate: generatePost, isPending: generatingPost } =
        useGeneratePostDraft(newsItemId);

    const { mutate: generateDiscussion, isPending: generatingDiscussion } =
        useGenerateDiscussionPrompt(newsItemId);

    const { mutate: saveDraft, isPending: saving } = useUpdateDraft(
        draft?.id ?? "",
        newsItemId
    );

    const [title, setTitle] = useState("");
    const [postContent, setPostContent] = useState("");
    const [discussionContent, setDiscussionContent] = useState("");
    const [isPublished, setIsPublished] = useState(false);

    // Tracks which generator was triggered so we can assign incoming draft to the correct field.
    const [pendingGeneration, setPendingGeneration] = useState(null); // "post" | "discussion" | null

    // Sync local state when draft loads/changes.
    useEffect(() => {
        if (!draft) return;

        queueMicrotask(() => {
            // If a generation was just triggered, populate only the corresponding field.
            if (pendingGeneration === "post") {
                setTitle(draft.title ?? title);
                // Try to use draft.content directly for post drafts.
                setPostContent(draft.content ?? "");
                setPendingGeneration(null);
                return;
            }

            if (pendingGeneration === "discussion") {
                // Discussion generator returns a draft; set discussion content.
                setDiscussionContent(draft.content ?? "");
                setPendingGeneration(null);
                return;
            }

            // Default initialization when loading an existing draft:
            setTitle(draft.title ?? "");
            // If content was previously saved for both parts, try to split by marker.
            const content = draft.content ?? "";
            const marker = "\n\nDISCUSSION:\n";
            if (content.includes(marker)) {
                const [postPart, discussionPart] = content.split(marker);
                // Remove possible "POST:" prefix if present
                setPostContent(postPart.replace(/^POST:\s*/i, "").trim());
                setDiscussionContent(discussionPart.trim());
            } else {
                // No marker — put everything into post content, keep discussion empty.
                setPostContent(content);
                setDiscussionContent("");
            }

            setIsPublished(draft.isPublished);
        });
    }, [draft]); // eslint-disable-line react-hooks/exhaustive-deps

    function handleSave() {
        // Combine both fields into single content for backend compatibility.
        const combinedContent = `POST:\n${postContent.trim()}\n\nDISCUSSION:\n${discussionContent.trim()}`;

        saveDraft({
            title,
            content: combinedContent,
            isPublished,
        });
    }

    function handleGeneratePost() {
        setPendingGeneration("post");
        generatePost();
    }

    function handleGenerateDiscussion() {
        setPendingGeneration("discussion");
        generateDiscussion();
    }

    return (
        <div className="border rounded p-4 bg-white">
            <h3 className="font-semibold mb-3">Szkic posta / dyskusji</h3>

            {error && (
                <div className="text-sm text-gray-600 mb-3">
                    Brak zapisanego szkicu. Wygeneruj post lub dyskusję.
                </div>
            )}

            {isLoading && (
                <div className="text-sm text-gray-600 mb-3">
                    Ładowanie szkicu...
                </div>
            )}

            {!isLoading && (
                <div className="space-y-3">
                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">
                            Tytuł (post)
                        </label>
                        <input
                            type="text"
                            className="w-full border rounded px-3 py-2 text-sm"
                            value={title}
                            onChange={(e) => setTitle(e.target.value)}
                        />
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">
                            Treść posta
                        </label>

                        {postContent.trim() !== "" && (
                            <textarea
                                className="w-3/4 border rounded px-3 py-2 text-sm"
                                rows={10}
                                cols={100}
                                value={postContent}
                                onChange={(e) => setPostContent(e.target.value)}
                            />
                        )}
                        <div className="flex gap-2 mt-2">
                            <button
                                className="px-3 py-1.5 bg-green-600 text-white rounded text-sm hover:bg-green-700 disabled:opacity-50"
                                onClick={handleGeneratePost}
                                disabled={generatingPost}
                            >
                                {generatingPost ? "Generowanie..." : "Generuj post"}
                            </button>
                        </div>
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">
                            Treść dyskusji
                        </label>

                        {discussionContent.trim() !== "" && (
                            <textarea
                                className="w-3/4 border rounded px-3 py-2 text-sm"
                                rows={10}
                                cols={100}
                                value={discussionContent}
                                onChange={(e) => setDiscussionContent(e.target.value)}
                            />
                        )}
                        <div className="flex gap-2 mt-2">
                            <button
                                className="px-3 py-1.5 bg-purple-600 text-white rounded text-sm hover:bg-purple-700 disabled:opacity-50"
                                onClick={handleGenerateDiscussion}
                                disabled={generatingDiscussion}
                            >
                                {generatingDiscussion ? "Generowanie..." : "Generuj dyskusję"}
                            </button>
                        </div>
                    </div>

                    <div className="flex items-center gap-2">
                        <input
                            id="isPublished"
                            type="checkbox"
                            checked={isPublished}
                            onChange={(e) => setIsPublished(e.target.checked)}
                            className="h-4 w-4"
                        />
                        <label htmlFor="isPublished" className="text-sm text-gray-700">
                            Opublikowany
                        </label>
                    </div>

                    <div className="flex flex-wrap gap-2">
                        <button
                            className="px-3 py-1.5 bg-blue-600 text-white rounded text-sm hover:bg-blue-700 disabled:opacity-50"
                            onClick={handleSave}
                            disabled={saving}
                        >
                            {saving ? "Zapisywanie..." : "Zapisz"}
                        </button>
                    </div>

                    {draft?.sourceUrl && (
                        <div className="text-xs text-gray-500">
                            Źródło:{" "}
                            <a
                                href={draft.sourceUrl}
                                target="_blank"
                                rel="noreferrer"
                                className="text-blue-600 underline"
                            >
                                {draft.sourceUrl}
                            </a>
                        </div>
                    )}
                </div>
            )}
        </div>
    );
}