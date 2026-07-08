using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Provides navigation history operations.
    /// </summary>
    public interface INavigationHistory
    {
        /// <summary>
        /// Gets the number of items in the navigation history.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets a value indicating whether there is a previous entry.
        /// </summary>
        bool CanGoBack { get; }

        /// <summary>
        /// Adds a ViewModel instance to the history.
        /// </summary>
        /// <param name="viewModel">The ViewModel to store.</param>
        void Push(object viewModel);

        /// <summary>
        /// Removes and returns the previous ViewModel.
        /// </summary>
        /// <returns>The previous ViewModel, or null if history is empty.</returns>
        object? Pop();

        /// <summary>
        /// Removes all entries from the history.
        /// </summary>
        void Clear();
    }
}