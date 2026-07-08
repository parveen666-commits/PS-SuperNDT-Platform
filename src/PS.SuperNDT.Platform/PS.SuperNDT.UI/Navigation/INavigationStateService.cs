using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for storing and retrieving navigation state.
    /// </summary>
    public interface INavigationStateService
    {
        /// <summary>
        /// Gets the current navigation state.
        /// </summary>
        INavigationState State { get; }

        /// <summary>
        /// Updates the current navigation state.
        /// </summary>
        /// <param name="viewModel">The current ViewModel.</param>
        void UpdateState(object? viewModel);

        /// <summary>
        /// Resets the navigation state.
        /// </summary>
        void Reset();
    }
}