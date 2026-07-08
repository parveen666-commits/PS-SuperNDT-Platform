using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationCancellationService"/>.
    /// </summary>
    public sealed class NavigationCancellationService : INavigationCancellationService
    {
        /// <inheritdoc/>
        public bool IsCancelled { get; private set; }

        /// <inheritdoc/>
        public void Cancel()
        {
            IsCancelled = true;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            IsCancelled = false;
        }
    }
}