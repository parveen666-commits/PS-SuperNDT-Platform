using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for navigation health monitoring.
    /// </summary>
    public interface INavigationHealthService
    {
        /// <summary>
        /// Gets whether navigation system is healthy.
        /// </summary>
        bool IsHealthy { get; }

        /// <summary>
        /// Gets the latest health message.
        /// </summary>
        string? StatusMessage { get; }

        /// <summary>
        /// Updates navigation health state.
        /// </summary>
        /// <param name="healthy">Health status.</param>
        /// <param name="message">Status message.</param>
        void Update(bool healthy, string? message = null);

        /// <summary>
        /// Resets health state.
        /// </summary>
        void Reset();
    }
}