using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationStateChangeService"/>.
    /// </summary>
    public sealed class NavigationStateChangeService : INavigationStateChangeService
    {
        /// <inheritdoc/>
        public DateTime? LastChanged { get; private set; }

        /// <inheritdoc/>
        public void NotifyChanged()
        {
            LastChanged = DateTime.UtcNow;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            LastChanged = null;
        }
    }
}