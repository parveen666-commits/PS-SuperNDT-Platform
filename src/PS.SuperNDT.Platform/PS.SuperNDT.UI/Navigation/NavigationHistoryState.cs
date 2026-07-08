using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationHistoryState"/>.
    /// </summary>
    public sealed class NavigationHistoryState : INavigationHistoryState
    {
        /// <inheritdoc/>
        public bool CanNavigateBack { get; private set; }

        /// <inheritdoc/>
        public bool CanNavigateForward { get; private set; }

        /// <inheritdoc/>
        public void Update(bool canBack, bool canForward)
        {
            CanNavigateBack = canBack;
            CanNavigateForward = canForward;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            CanNavigateBack = false;
            CanNavigateForward = false;
        }
    }
}