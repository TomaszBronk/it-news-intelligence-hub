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
        <div className="draft-editor">
        

            {error && (
                <div className="draft-status-message">
                    No saved draft yet. Generate a post or discussion prompt
                </div>
            )}

            {isLoading && (
                <div className="draft-status-message">
                    Loading draft...
                </div>
            )}

            {!isLoading  && (
                <div className="draft-form">
                    <div className="draft-field">
                        <label htmlFor={`draft-title-${newsItemId}`}>
                            Title
                        </label>
                        <input
                            id={`draft-title-${newsItemId}`}
                            type="text"
                            value={title}
                            onChange={(event) =>
                                setTitle(event.target.value)
                            }
                        />
                    </div>

                    <div className="draft-field">
                        <label htmlFor={`draft-content-${newsItemId}`}>
                            Content
                        </label>

                        {postContent.trim() !== "" && (
                            <textarea
                                id={`draft-content-${newsItemId}`}
                                value={postContent}
                                onChange={(e) => setPostContent(e.target.value)}
                            />
                        )}
                        <div className="flex gap-2 mt-2">
                            <button
                                className="news-action-button button-secondary"
                                onClick={handleGeneratePost}
                                disabled={generatingPost}
                            >
                                {generatingPost ? "Generating..." : "Generate post"}
                            </button>
                        </div>
                    </div>

                  

                    <div className="draft-field">
                            <label htmlFor={`draft-content-${newsItemId}`}>
                                Discussion
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
                                className="news-action-button button-secondary"
                                onClick={handleGenerateDiscussion}
                                disabled={generatingDiscussion}
                            >
                                {generatingDiscussion ? "Generating..." : "Generate discussion"}
                            </button>
                        </div>
                    </div>

                    <div className="draft-published-checkbox">
                        <input
                            id="isPublished"
                            type="checkbox"
                            checked={isPublished}
                            onChange={(e) => setIsPublished(e.target.checked)}
                            className="h-4 w-4"
                        />
                        <label htmlFor="isPublished" className="text-sm text-gray-700">
                            Publish
                        </label>
                    </div>

                    <div className="draft-actions">
                        <button
                            className="news-action-button button-primary"
                            onClick={handleSave}
                            disabled={saving}
                        >
                            {saving ? "Saving..." : "Save"}
                        </button>
                    </div>

                    {draft?.sourceUrl && (
                        <div className="draft-source">
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