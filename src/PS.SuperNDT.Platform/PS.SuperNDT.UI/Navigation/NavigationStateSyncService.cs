using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationStateSyncService"/>.
    /// </summary>
    public sealed class NavigationStateSyncService : INavigationStateSyncService
    {
        /// <inheritdoc/>
        public bool IsActive { get; private set; }

        /// <inheritdoc/>
        public void Synchronize()
        {
            if (!IsActive)
            {
                return;
            }

            // Navigation state synchronization logic can be implemented here.
        }

        /// <inheritdoc/>
        public void Activate()
        {
            IsActive = true;
        }

        /// <inheritdoc/>
        public void Deactivate()
        {
            IsActive = false;
        }
    }
}