using System;
using System.Collections.Generic;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationRedirectService"/>.
    /// </summary>
    public sealed class NavigationRedirectService : INavigationRedirectService
    {
        private readonly Dictionary<Type, Type> _redirects = new();

        /// <inheritdoc/>
        public void AddRedirect(Type source, Type target)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(target);

            _redirects[source] = target;
        }

        /// <inheritdoc/>
        public Type? Resolve(Type source)
        {
            ArgumentNullException.ThrowIfNull(source);

            return _redirects.TryGetValue(source, out var target)
                ? target
                : null;
        }
    }
}