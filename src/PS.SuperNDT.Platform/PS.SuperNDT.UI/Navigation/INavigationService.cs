using System;
using System.Threading.Tasks;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines navigation operations for the application.
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Gets the currently active view model.
        /// </summary>
        object? CurrentViewModel { get; }

        /// <summary>
        /// Raised whenever the current view changes.
        /// </summary>
        event EventHandler? CurrentViewChanged;

        /// <summary>
        /// Navigates to the specified view model.
        /// </summary>
        /// <typeparam name="TViewModel">Target view model type.</typeparam>
        void NavigateTo<TViewModel>() where TViewModel : class;

        /// <summary>
        /// Navigates to the specified view model asynchronously.
        /// </summary>
        /// <typeparam name="TViewModel">Target view model type.</typeparam>
        Task NavigateToAsync<TViewModel>() where TViewModel : class;

        /// <summary>
        /// Returns true if back navigation is available.
        /// </summary>
        bool CanGoBack { get; }

        /// <summary>
        /// Navigates to the previous view if available.
        /// </summary>
        void GoBack();

        /// <summary>
        /// Clears the navigation history.
        /// </summary>
        void ClearHistory();
    }
}