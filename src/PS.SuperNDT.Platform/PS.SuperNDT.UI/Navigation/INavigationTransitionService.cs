using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for controlling navigation transitions.
    /// </summary>
    public interface INavigationTransitionService
    {
        /// <summary>
        /// Gets whether a transition is currently running.
        /// </summary>
        bool IsTransitioning { get; }

        /// <summary>
        /// Starts a navigation transition.
        /// </summary>
        void Begin();

        /// <summary>
        /// Completes a navigation transition.
        /// </summary>
        void End();
    }
}