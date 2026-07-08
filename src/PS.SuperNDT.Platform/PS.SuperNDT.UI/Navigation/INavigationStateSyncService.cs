using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for handling navigation state synchronization events.
    /// </summary>
    public interface INavigationStateSyncService
    {
        /// <summary>
        /// Gets whether state synchronization is active.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Synchronizes navigation state.
        /// </summary>
        void Synchronize();

        /// <summary>
        /// Activates synchronization.
        /// </summary>
        void Activate();

        /// <summary>
        /// Deactivates synchronization.
        /// </summary>
        void Deactivate();
    }
}