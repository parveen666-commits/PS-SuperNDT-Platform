using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for managing navigation synchronization state.
    /// </summary>
    public interface INavigationSynchronizationState
    {
        /// <summary>
        /// Gets the last synchronization time.
        /// </summary>
        DateTime? LastSyncTime { get; }

        /// <summary>
        /// Gets whether synchronization was successful.
        /// </summary>
        bool IsSynchronized { get; }

        /// <summary>
        /// Updates synchronization state.
        /// </summary>
        /// <param name="success">Synchronization result.</param>
        void Update(bool success);

        /// <summary>
        /// Clears synchronization state.
        /// </summary>
        void Reset();
    }
}