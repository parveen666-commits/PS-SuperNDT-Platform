using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationMonitor"/>.
    /// </summary>
    public sealed class NavigationMonitor : INavigationMonitor
    {
        /// <inheritdoc/>
        public bool IsNavigating { get; private set; }

        /// <inheritdoc/>
        public void Start()
        {
            IsNavigating = true;
        }

        /// <inheritdoc/>
        public void Stop()
        {
            IsNavigating = false;
        }
    }
}