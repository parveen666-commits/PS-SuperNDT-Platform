using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationLifecycleService"/>.
    /// </summary>
    public sealed class NavigationLifecycleService : INavigationLifecycleService
    {
        /// <inheritdoc/>
        public bool IsActive { get; private set; }

        /// <inheritdoc/>
        public void Start()
        {
            IsActive = true;
        }

        /// <inheritdoc/>
        public void Stop()
        {
            IsActive = false;
        }
    }
}