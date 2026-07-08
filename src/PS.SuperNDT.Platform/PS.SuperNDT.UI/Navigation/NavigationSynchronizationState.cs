using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationSynchronizationState"/>.
    /// </summary>
    public sealed class NavigationSynchronizationState : INavigationSynchronizationState
    {
        /// <inheritdoc/>
        public DateTime? LastSyncTime { get; private set; }

        /// <inheritdoc/>
        public bool IsSynchronized { get; private set; }

        /// <inheritdoc/>
        public void Update(bool success)
        {
            IsSynchronized = success;
            LastSyncTime = DateTime.UtcNow;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            LastSyncTime = null;
            IsSynchronized = false;
        }
    }
}