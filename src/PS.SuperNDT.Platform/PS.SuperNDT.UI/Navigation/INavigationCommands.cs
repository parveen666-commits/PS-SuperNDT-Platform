using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines navigation commands available to the UI.
    /// </summary>
    public interface INavigationCommands
    {
        /// <summary>
        /// Navigates to a ViewModel.
        /// </summary>
        /// <typeparam name="TViewModel">Target ViewModel type.</typeparam>
        void Navigate<TViewModel>() where TViewModel : class;

        /// <summary>
        /// Navigates back to the previous ViewModel.
        /// </summary>
        void GoBack();

        /// <summary>
        /// Determines whether back navigation is available.
        /// </summary>
        bool CanGoBack { get; }

        /// <summary>
        /// Clears navigation history.
        /// </summary>
        void ClearHistory();
    }
}