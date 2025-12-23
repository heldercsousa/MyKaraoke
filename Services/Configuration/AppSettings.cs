namespace MyVocaList.Services.Configuration
{
    /// <summary>
    /// Root application settings loaded from appsettings.json
    /// Structured to support future configuration sections as the app evolves
    /// </summary>
    public class AppSettings
    {
        public PaginationSettings Pagination { get; set; } = new();

        // Future configuration sections will be added here as needed:
        // public UiSettings UI { get; set; } = new();
        // public DatabaseSettings Database { get; set; } = new();
        // public NotificationSettings Notifications { get; set; } = new();
        // public CacheSettings Cache { get; set; } = new();
    }

    /// <summary>
    /// Pagination configuration for list views across the application
    /// These settings optimize memory usage and UI performance on mobile devices
    /// </summary>
    public class PaginationSettings
    {
        /// <summary>
        /// Number of items to load per page request
        /// Default: 30 items (optimal for most mobile screens)
        /// Range: 20-50 recommended for mobile performance
        /// </summary>
        public int PageSize { get; set; } = 30;

        /// <summary>
        /// Maximum number of items to keep in memory before cleanup
        /// Default: 100 items (prevents excessive memory usage)
        /// When limit is exceeded, oldest items are removed from the collection
        /// </summary>
        public int MaxItemsInMemory { get; set; } = 100;

        /// <summary>
        /// Threshold for triggering "load more" operation
        /// Default: 5 items (load next page when user is 5 items from bottom)
        /// Lower values = more aggressive prefetching
        /// </summary>
        public int LoadMoreThreshold { get; set; } = 5;

        /// <summary>
        /// Delay in milliseconds before executing "load more" to prevent rapid-fire requests
        /// Default: 300ms (prevents loading during fast scrolling)
        /// </summary>
        public int LoadMoreDebounceMs { get; set; } = 300;

        /// <summary>
        /// Number of items to initially load on page open
        /// Computed from PageSize for consistency
        /// </summary>
        public int InitialLoadCount => PageSize;
    }
}
