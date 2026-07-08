using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for navigation retry handling.
    /// </summary>
    public interface INavigationRetryService
    {
        /// <summary>
        /// Gets the maximum retry count.
        /// </summary>
        int MaxRetries { get; }

        /// <summary>
        /// Gets the current retry count.
        /// </summary>
        int RetryCount { get; }

        /// <summary>
        /// Determines whether another retry is allowed.
        /// </summary>
        bool CanRetry { get; }

        /// <summary>
        /// Increases retry count.
        /// </summary>
        void Increment();

        /// <summary>
        /// Resets retry count.
        /// </summary>
        void Reset();
    }
}