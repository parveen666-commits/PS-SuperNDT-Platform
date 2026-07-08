using System;
using System.Collections.Generic;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationParameterService"/>.
    /// </summary>
    public sealed class NavigationParameterService : INavigationParameterService
    {
        private readonly Dictionary<string, object?> _parameters =
            new(StringComparer.Ordinal);

        /// <inheritdoc/>
        public void Set(string key, object? value)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);

            _parameters[key] = value;
        }

        /// <inheritdoc/>
        public object? Get(string key)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);

            return _parameters.TryGetValue(key, out var value)
                ? value
                : null;
        }

        /// <inheritdoc/>
        public void Remove(string key)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);

            _parameters.Remove(key);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _parameters.Clear();
        }
    }
}