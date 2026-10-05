export function createEmptyFilters() {
    return {
        sourceId: '',
        category: '',
        status: '',
        publishedFromUtc: '',
        publishedToUtc: '',
        search: '',
        page: 1,
        pageSize: 50,
    };
}

export function normalizeFilters(filters) {
    return {
        sourceId: filters.sourceId || undefined,
        category: filters.category || undefined,
        status: filters.status || undefined,
        publishedFromUtc: toStartOfDayUtc(filters.publishedFromUtc),
        publishedToUtc: toEndOfDayUtc(filters.publishedToUtc),
        search: filters.search.trim() || undefined,
        page: 1,
        pageSize: 50,
    };
}

function toStartOfDayUtc(value) {
    return value
        ? `${value}T00:00:00Z`
        : undefined;
}

function toEndOfDayUtc(value) {
    return value
        ? `${value}T23:59:59.999Z`
        : undefined;
}