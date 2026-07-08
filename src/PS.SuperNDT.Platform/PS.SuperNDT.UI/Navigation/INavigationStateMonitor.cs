using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for navigation state monitoring.
    /// </summary>
    public interface INavigationStateMonitor
    {
        /// <summary>
        /// Gets whether the navigation state is valid.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Updates the monitored navigation state.
        /// </summary>
        /// <param name="state">Navigation state.</param>
        void Update(INavigationState state);

        /// <summary>
        /// Resets monitoring state.
        /// </summary>
        void Reset();
    }
}