using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationPauseService"/>.
    /// </summary>
    public sealed class NavigationPauseService : INavigationPauseService
    {
        /// <inheritdoc/>
        public bool IsPaused { get; private set; }

        /// <inheritdoc/>
        public void Pause()
        {
            IsPaused = true;
        }

        /// <inheritdoc/>
        public void Resume()
        {
            IsPaused = false;
        }
    }
}