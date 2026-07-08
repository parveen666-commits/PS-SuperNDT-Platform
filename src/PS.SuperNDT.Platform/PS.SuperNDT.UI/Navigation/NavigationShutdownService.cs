using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationShutdownService"/>.
    /// </summary>
    public sealed class NavigationShutdownService : INavigationShutdownService
    {
        /// <inheritdoc/>
        public bool IsShutdown { get; private set; }

        /// <inheritdoc/>
        public void Shutdown()
        {
            IsShutdown = true;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            IsShutdown = false;
        }
    }
}