using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for pausing and resuming navigation processing.
    /// </summary>
    public interface INavigationPauseService
    {
        /// <summary>
        /// Gets whether navigation is paused.
        /// </summary>
        bool IsPaused { get; }

        /// <summary>
        /// Pauses navigation.
        /// </summary>
        void Pause();

        /// <summary>
        /// Resumes navigation.
        /// </summary>
        void Resume();
    }
}