using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for navigation state synchronization.
    /// </summary>
    public interface INavigationSyncService
    {
        /// <summary>
        /// Gets whether synchronization is enabled.
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Enables navigation synchronization.
        /// </summary>
        void Enable();

        /// <summary>
        /// Disables navigation synchronization.
        /// </summary>
        void Disable();

        /// <summary>
        /// Synchronizes the current navigation state.
        /// </summary>
        void Sync();
    }
}