using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationState"/>.
    /// </summary>
    public sealed class NavigationState : INavigationState
    {
        /// <inheritdoc/>
        public object? CurrentViewModel { get; private set; }

        /// <inheritdoc/>
        public object? PreviousViewModel { get; private set; }

        /// <inheritdoc/>
        public DateTime? LastNavigationTime { get; private set; }

        /// <summary>
        /// Updates the navigation state.
        /// </summary>
        /// <param name="currentViewModel">The new current ViewModel.</param>
        public void Update(object? currentViewModel)
        {
            PreviousViewModel = CurrentViewModel;
            CurrentViewModel = currentViewModel;
            LastNavigationTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Clears the navigation state.
        /// </summary>
        public void Reset()
        {
            PreviousViewModel = null;
            CurrentViewModel = null;
            LastNavigationTime = null;
        }
    }
}