const baseUrl = '/api/news-sources';

export class ApiError extends Error {
    constructor(message, status, details) {
        super(message);
        this.name = 'ApiError';
        this.status = status;
        this.details = details;
    }
}

export async function getNewsSources() {
    const response = await fetch(baseUrl);

    if (!response.ok) {
        throw await createApiError(response);
    }

    return response.json();
}

export async function createNewsSource(request) {
    const response = await fetch(baseUrl, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(request),
    });

    if (!response.ok) {
        throw await createApiError(response);
    }

    return response.json();
}

export async function fetchNewsSource(sourceId) {
    const response = await fetch(`${baseUrl}/${sourceId}/fetch`, {
        method: 'POST',
    });

    if (!response.ok) {
        throw await createApiError(response);
    }

    return response.json();
}

async function createApiError(response) {
    let details;

    try {
        details = await response.json();
    } catch {
        details = undefined;
    }

    const validationErrors = details?.errors
        ? Object.values(details.errors).flat().join(' ')
        : undefined;

    const message =
        validationErrors ??
        details?.message ??
        details?.detail ??
        details?.title ??
        getDefaultErrorMessage(response.status);

    return new ApiError(message, response.status, details);
}

function getDefaultErrorMessage(status) {
    switch (status) {
        case 400:
            return 'The submitted data is invalid. Please review the form.';
        case 409:
            return 'A news source with this RSS feed URL already exists.';
        case 500:
            return 'The server encountered an unexpected error.';
        default:
            return 'Unable to complete the request. Please try again.';
    }
}