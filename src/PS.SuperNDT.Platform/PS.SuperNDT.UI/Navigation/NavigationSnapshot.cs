using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Represents a stored snapshot of the current navigation state.
    /// </summary>
    public sealed class NavigationSnapshot
    {
        public NavigationSnapshot(
            object? currentViewModel,
            object? previousViewModel,
            DateTime timestamp)
        {
            CurrentViewModel = currentViewModel;
            PreviousViewModel = previousViewModel;
            Timestamp = timestamp;
        }

        /// <summary>
        /// Gets the current ViewModel.
        /// </summary>
        public object? CurrentViewModel { get; }

        /// <summary>
        /// Gets the previous ViewModel.
        /// </summary>
        public object? PreviousViewModel { get; }

        /// <summary>
        /// Gets the snapshot creation time.
        /// </summary>
        public DateTime Timestamp { get; }
    }
}