namespace MyVocaList.Services.Configuration
{
    /// <summary>
    /// Global pagination settings for list pages across the application
    /// These settings optimize memory usage and UI performance on mobile devices
    /// </summary>
    public static class PaginationSettings
    {
        /// <summary>
        /// Number of items to load per page request
        /// Default: 30 items (optimal for most mobile screens)
        /// </summary>
        public static int PageSize { get; set; } = 30;

        /// <summary>
        /// Maximum number of items to keep in memory before cleanup
        /// Default: 100 items (prevents excessive memory usage)
        /// When limit is reached, older pages are removed from memory
        /// </summary>
        public static int MaxItemsInMemory { get; set; } = 100;

        /// <summary>
        /// Threshold for triggering "load more" operation
        /// Default: 5 items (load next page when user is 5 items from bottom)
        /// </summary>
        public static int LoadMoreThreshold { get; set; } = 5;

        /// <summary>
        /// Delay in milliseconds before executing "load more" to prevent rapid-fire requests
        /// Default: 300ms (prevents loading during fast scrolling)
        /// </summary>
        public static int LoadMoreDebounceMs { get; set; } = 300;

        /// <summary>
        /// Number of items to initially load on page open
        /// Default: Same as PageSize for consistency
        /// </summary>
        public static int InitialLoadCount => PageSize;
    }
}
