using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationSyncService"/>.
    /// </summary>
    public sealed class NavigationSyncService : INavigationSyncService
    {
        /// <inheritdoc/>
        public bool IsEnabled { get; private set; }

        /// <inheritdoc/>
        public void Enable()
        {
            IsEnabled = true;
        }

        /// <inheritdoc/>
        public void Disable()
        {
            IsEnabled = false;
        }

        /// <inheritdoc/>
        public void Sync()
        {
            if (!IsEnabled)
            {
                return;
            }

            // Synchronization logic can be implemented here.
        }
    }
}