using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationStartupService"/>.
    /// </summary>
    public sealed class NavigationStartupService : INavigationStartupService
    {
        /// <inheritdoc/>
        public bool IsInitialized { get; private set; }

        /// <inheritdoc/>
        public void Initialize()
        {
            IsInitialized = true;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            IsInitialized = false;
        }
    }
}