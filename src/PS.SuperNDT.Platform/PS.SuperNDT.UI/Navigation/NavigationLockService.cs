using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationLockService"/>.
    /// </summary>
    public sealed class NavigationLockService : INavigationLockService
    {
        /// <inheritdoc/>
        public bool IsLocked { get; private set; }

        /// <inheritdoc/>
        public void Lock()
        {
            IsLocked = true;
        }

        /// <inheritdoc/>
        public void Unlock()
        {
            IsLocked = false;
        }
    }
}