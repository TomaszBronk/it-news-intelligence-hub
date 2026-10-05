import { useState } from 'react';

const categories = [
    'DotNet',
    'Azure',
    'AI',
    'Security',
    'React',
    'DevOps',
    'Data',
    'Other',
];

const initialFormState = {
    name: '',
    feedUrl: '',
    websiteUrl: '',
    category: 'Other',
    isActive: true,
};

export function NewsSourceForm({ isSubmitting, onSubmit, onCancel }) {
    const [form, setForm] = useState(initialFormState);
    const [validationError, setValidationError] = useState(null);

    async function handleSubmit(event) {
        event.preventDefault();
        setValidationError(null);

        const name = form.name.trim();
        const feedUrl = form.feedUrl.trim();
        const websiteUrl = form.websiteUrl.trim();

        if (!name) {
            setValidationError('Source name is required.');
            return;
        }

        if (!isValidHttpUrl(feedUrl)) {
            setValidationError('Feed URL must be a valid HTTP or HTTPS URL.');
            return;
        }

        if (websiteUrl && !isValidHttpUrl(websiteUrl)) {
            setValidationError('Website URL must be a valid HTTP or HTTPS URL.');
            return;
        }

        await onSubmit({
            ...form,
            name,
            feedUrl,
            websiteUrl: websiteUrl || undefined,
        });

        setForm(initialFormState);
    }

    return (
        <form className="news-source-form" onSubmit={handleSubmit}>
            <div className="form-heading">
                <div>
                    <p className="eyebrow">Configuration</p>
                    <h2>Add news source</h2>
                </div>

                <button
                    className="button button-secondary"
                    type="button"
                    onClick={onCancel}
                    disabled={isSubmitting}
                >
                    Cancel
                </button>
            </div>

            {validationError && (
                <p className="form-message form-message-error" role="alert">
                    {validationError}
                </p>
            )}

            <div className="form-grid">
                <label>
                    <span>Source name</span>
                    <input
                        type="text"
                        value={form.name}
                        maxLength={150}
                        placeholder="e.g. Microsoft .NET Blog"
                        onChange={(event) =>
                            setForm((current) => ({
                                ...current,
                                name: event.target.value,
                            }))
                        }
                        disabled={isSubmitting}
                        required
                    />
                </label>

                <label>
                    <span>Category</span>
                    <select
                        value={form.category}
                        onChange={(event) =>
                            setForm((current) => ({
                                ...current,
                                category: event.target.value,
                            }))
                        }
                        disabled={isSubmitting}
                    >
                        {categories.map((category) => (
                            <option key={category} value={category}>
                                {category}
                            </option>
                        ))}
                    </select>
                </label>

                <label className="form-field-wide">
                    <span>RSS or Atom feed URL</span>
                    <input
                        type="url"
                        value={form.feedUrl}
                        maxLength={2048}
                        placeholder="https://example.com/feed.xml"
                        onChange={(event) =>
                            setForm((current) => ({
                                ...current,
                                feedUrl: event.target.value,
                            }))
                        }
                        disabled={isSubmitting}
                        required
                    />
                </label>

                <label className="form-field-wide">
                    <span>Website URL (optional)</span>
                    <input
                        type="url"
                        value={form.websiteUrl}
                        maxLength={2048}
                        placeholder="https://example.com"
                        onChange={(event) =>
                            setForm((current) => ({
                                ...current,
                                websiteUrl: event.target.value,
                            }))
                        }
                        disabled={isSubmitting}
                    />
                </label>

                <label className="checkbox-field form-field-wide">
                    <input
                        type="checkbox"
                        checked={form.isActive}
                        onChange={(event) =>
                            setForm((current) => ({
                                ...current,
                                isActive: event.target.checked,
                            }))
                        }
                        disabled={isSubmitting}
                    />
                    <span>Enable scheduled news collection for this source</span>
                </label>
            </div>

            <div className="form-actions">
                <button
                    className="button button-primary"
                    type="submit"
                    disabled={isSubmitting}
                >
                    {isSubmitting ? 'Saving source...' : 'Save source'}
                </button>
            </div>
        </form>
    );
}

function isValidHttpUrl(value) {
    try {
        const url = new URL(value);

        return url.protocol === 'http:' || url.protocol === 'https:';
    } catch {
        return false;
    }
}