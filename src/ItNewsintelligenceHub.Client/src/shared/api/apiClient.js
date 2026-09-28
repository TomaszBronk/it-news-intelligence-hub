import axios from 'axios';

export class ApiError extends Error {
    constructor(message, status, details) {
        super(message);
        this.name = 'ApiError';
        this.status = status;
        this.details = details;
    }
}

export const apiClient = axios.create({
    baseURL: '/api',
    timeout: 30000,
    headers: {
        Accept: 'application/json',
    },
});

apiClient.interceptors.response.use(
    (response) => response,
    (error) => {
        const status = error.response?.status ?? 0;
        const details = error.response?.data;

        const validationErrors = details?.errors
            ? Object.values(details.errors).flat().join(' ')
            : undefined;

        const message =
            validationErrors ??
            details?.message ??
            details?.detail ??
            details?.title ??
            getFallbackMessage(status, error.code);

        return Promise.reject(new ApiError(message, status, details));
    },
);

function getFallbackMessage(status, errorCode) {
    if (errorCode === 'ECONNABORTED') {
        return 'The request timed out. Please try again.';
    }

    switch (status) {
        case 0:
            return 'Unable to connect to the API. Please check that the backend is running.';
        case 400:
            return 'The submitted data is invalid.';
        case 401:
            return 'Authentication is required.';
        case 403:
            return 'You do not have permission to perform this action.';
        case 404:
            return 'The requested resource was not found.';
        case 409:
            return 'The request conflicts with existing data.';
        case 422:
            return 'The request could not be processed.';
        case 500:
            return 'The server encountered an unexpected error.';
        default:
            return 'Unable to complete the request. Please try again.';
    }
}