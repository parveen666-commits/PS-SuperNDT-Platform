using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="IActiveNavigationScope"/>.
    /// </summary>
    public sealed class ActiveNavigationScope : IActiveNavigationScope
    {
        /// <inheritdoc/>
        public INavigationScope? CurrentScope { get; private set; }

        /// <inheritdoc/>
        public void SetScope(INavigationScope scope)
        {
            ArgumentNullException.ThrowIfNull(scope);

            CurrentScope = scope;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            CurrentScope = null;
        }
    }
}