using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for handling navigation cancellation.
    /// </summary>
    public interface INavigationCancellationService
    {
        /// <summary>
        /// Gets whether the current navigation has been cancelled.
        /// </summary>
        bool IsCancelled { get; }

        /// <summary>
        /// Cancels the current navigation operation.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Clears the cancellation state.
        /// </summary>
        void Reset();
    }
}