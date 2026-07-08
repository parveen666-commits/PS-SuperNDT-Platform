using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationTransitionService"/>.
    /// </summary>
    public sealed class NavigationTransitionService : INavigationTransitionService
    {
        /// <inheritdoc/>
        public bool IsTransitioning { get; private set; }

        /// <inheritdoc/>
        public void Begin()
        {
            IsTransitioning = true;
        }

        /// <inheritdoc/>
        public void End()
        {
            IsTransitioning = false;
        }
    }
}