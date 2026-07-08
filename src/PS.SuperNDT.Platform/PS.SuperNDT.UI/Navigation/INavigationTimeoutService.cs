using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for navigation timeout handling.
    /// </summary>
    public interface INavigationTimeoutService
    {
        /// <summary>
        /// Gets the configured timeout duration.
        /// </summary>
        TimeSpan Timeout { get; }

        /// <summary>
        /// Determines whether the timeout has expired.
        /// </summary>
        bool IsExpired(DateTime startTime);

        /// <summary>
        /// Updates the timeout duration.
        /// </summary>
        /// <param name="timeout">New timeout duration.</param>
        void SetTimeout(TimeSpan timeout);
    }
}