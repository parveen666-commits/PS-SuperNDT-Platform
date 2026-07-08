using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for monitoring navigation activity.
    /// </summary>
    public interface INavigationMonitor
    {
        /// <summary>
        /// Gets whether navigation is currently in progress.
        /// </summary>
        bool IsNavigating { get; }

        /// <summary>
        /// Starts navigation monitoring.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops navigation monitoring.
        /// </summary>
        void Stop();
    }
}