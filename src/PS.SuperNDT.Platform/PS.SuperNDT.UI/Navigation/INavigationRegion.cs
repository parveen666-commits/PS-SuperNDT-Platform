using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for tracking the active navigation region.
    /// </summary>
    public interface INavigationRegion
    {
        /// <summary>
        /// Gets the region name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the current View displayed in the region.
        /// </summary>
        object? CurrentView { get; }

        /// <summary>
        /// Gets the current ViewModel displayed in the region.
        /// </summary>
        object? CurrentViewModel { get; }

        /// <summary>
        /// Displays a View and its ViewModel in the region.
        /// </summary>
        /// <param name="view">The View instance.</param>
        /// <param name="viewModel">The ViewModel instance.</param>
        void SetContent(object view, object viewModel);

        /// <summary>
        /// Clears the region content.
        /// </summary>
        void Clear();
    }
}