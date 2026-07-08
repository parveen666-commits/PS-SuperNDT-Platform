using System;
using System.Collections.Generic;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationCacheService"/>.
    /// </summary>
    public sealed class NavigationCacheService : INavigationCacheService
    {
        private readonly Dictionary<string, object> _cache =
            new(StringComparer.OrdinalIgnoreCase);

        /// <inheritdoc/>
        public object? Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }

            return _cache.TryGetValue(key, out var value)
                ? value
                : null;
        }

        /// <inheritdoc/>
        public void Set(string key, object view)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(key);
            ArgumentNullException.ThrowIfNull(view);

            _cache[key] = view;
        }

        /// <inheritdoc/>
        public void Remove(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                _cache.Remove(key);
            }
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _cache.Clear();
        }
    }
}