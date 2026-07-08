using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationCommandState"/>.
    /// </summary>
    public sealed class NavigationCommandState : INavigationCommandState
    {
        /// <inheritdoc/>
        public bool CanExecute { get; private set; }

        /// <inheritdoc/>
        public void Update(bool canExecute)
        {
            CanExecute = canExecute;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            CanExecute = false;
        }
    }
}