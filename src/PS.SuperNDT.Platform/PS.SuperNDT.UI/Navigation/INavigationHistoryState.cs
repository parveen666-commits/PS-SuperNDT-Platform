using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for tracking navigation history state.
    /// </summary>
    public interface INavigationHistoryState
    {
        /// <summary>
        /// Gets whether history navigation is available.
        /// </summary>
        bool CanNavigateBack { get; }

        /// <summary>
        /// Gets whether forward navigation is available.
        /// </summary>
        bool CanNavigateForward { get; }

        /// <summary>
        /// Updates history navigation state.
        /// </summary>
        /// <param name="canBack">Back navigation availability.</param>
        /// <param name="canForward">Forward navigation availability.</param>
        void Update(bool canBack, bool canForward);

        /// <summary>
        /// Resets history state.
        /// </summary>
        void Reset();
    }
}