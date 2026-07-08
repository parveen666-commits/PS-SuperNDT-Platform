using System;
using System.Collections.Generic;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationScopeManager"/>.
    /// </summary>
    public sealed class NavigationScopeManager : INavigationScopeManager
    {
        private readonly Dictionary<string, INavigationScope> _scopes =
            new(StringComparer.OrdinalIgnoreCase);

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, INavigationScope> Scopes => _scopes;

        /// <inheritdoc/>
        public void Register(INavigationScope scope)
        {
            ArgumentNullException.ThrowIfNull(scope);

            _scopes[scope.Name] = scope;
        }

        /// <inheritdoc/>
        public INavigationScope? Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            return _scopes.TryGetValue(name, out var scope)
                ? scope
                : null;
        }

        /// <inheritdoc/>
        public void Remove(string name)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                _scopes.Remove(name);
            }
        }
    }
}