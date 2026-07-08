using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationAvailabilityService"/>.
    /// </summary>
    public sealed class NavigationAvailabilityService : INavigationAvailabilityService
    {
        /// <inheritdoc/>
        public bool IsAvailable { get; private set; } = true;

        /// <inheritdoc/>
        public void Enable()
        {
            IsAvailable = true;
        }

        /// <inheritdoc/>
        public void Disable()
        {
            IsAvailable = false;
        }
    }
}