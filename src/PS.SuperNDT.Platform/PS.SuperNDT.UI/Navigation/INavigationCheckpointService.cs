using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for handling navigation checkpoints.
    /// </summary>
    public interface INavigationCheckpointService
    {
        /// <summary>
        /// Creates a navigation checkpoint.
        /// </summary>
        void CreateCheckpoint();

        /// <summary>
        /// Restores the last navigation checkpoint.
        /// </summary>
        void RestoreCheckpoint();

        /// <summary>
        /// Clears the current checkpoint.
        /// </summary>
        void ClearCheckpoint();
    }
}