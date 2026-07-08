using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for managing navigation state snapshots.
    /// </summary>
    public interface INavigationSnapshotService
    {
        /// <summary>
        /// Creates a snapshot of the current navigation state.
        /// </summary>
        /// <returns>The navigation snapshot.</returns>
        NavigationSnapshot CreateSnapshot();

        /// <summary>
        /// Restores a previously created navigation snapshot.
        /// </summary>
        /// <param name="snapshot">The snapshot to restore.</param>
        void RestoreSnapshot(NavigationSnapshot snapshot);
    }
}