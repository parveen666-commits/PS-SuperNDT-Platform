using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Event arguments for navigation changes.
    /// </summary>
    public sealed class NavigationChangedEventArgs : EventArgs
    {
        public NavigationChangedEventArgs(object? previousViewModel, object? currentViewModel)
        {
            PreviousViewModel = previousViewModel;
            CurrentViewModel = currentViewModel;
        }

        /// <summary>
        /// Gets the previous ViewModel.
        /// </summary>
        public object? PreviousViewModel { get; }

        /// <summary>
        /// Gets the current ViewModel.
        /// </summary>
        public object? CurrentViewModel { get; }
    }
}