/**
 * @typedef {Object} NewsItem
 * @property {string} id
 * @property {string} sourceId
 * @property {string} sourceName
 * @property {string} title
 * @property {string | null} summary
 * @property {string} originalUrl
 * @property {string | null} author
 * @property {string | null} publishedAtUtc
 * @property {string} retrievedAtUtc
 * @property {string} category
 * @property {"New" | "Saved" | "Read" | "Archived"} status
 * @property {string | null} note
 */

/**
 * @typedef {Object} PostDraft
 * @property {string} id
 * @property {string} newsItemId
 * @property {string} title
 * @property {string} content
 * @property {"Post" | "DiscussionPrompt"} type
 * @property {string} sourceUrl
 * @property {boolean} isPublished
 * @property {string} createdAtUtc
 * @property {string | null} updatedAtUtc
 */

export { };