using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for caching navigation views.
    /// </summary>
    public interface INavigationCacheService
    {
        /// <summary>
        /// Gets a cached view instance.
        /// </summary>
        /// <param name="key">Cache key.</param>
        /// <returns>The cached view, if available.</returns>
        object? Get(string key);

        /// <summary>
        /// Adds a view instance to the cache.
        /// </summary>
        /// <param name="key">Cache key.</param>
        /// <param name="view">View instance.</param>
        void Set(string key, object view);

        /// <summary>
        /// Removes a cached view.
        /// </summary>
        /// <param name="key">Cache key.</param>
        void Remove(string key);

        /// <summary>
        /// Clears all cached views.
        /// </summary>
        void Clear();
    }
}