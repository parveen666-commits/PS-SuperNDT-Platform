using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for monitoring navigation history changes.
    /// </summary>
    public interface INavigationHistoryMonitor
    {
        /// <summary>
        /// Gets the current history count.
        /// </summary>
        int HistoryCount { get; }

        /// <summary>
        /// Updates the history count.
        /// </summary>
        /// <param name="count">Current history count.</param>
        void Update(int count);

        /// <summary>
        /// Resets the history monitor.
        /// </summary>
        void Reset();
    }
}