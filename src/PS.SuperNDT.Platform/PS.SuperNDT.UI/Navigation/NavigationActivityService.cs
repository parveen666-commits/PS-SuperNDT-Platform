using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationActivityService"/>.
    /// </summary>
    public sealed class NavigationActivityService : INavigationActivityService
    {
        /// <inheritdoc/>
        public DateTime? LastActivity { get; private set; }

        /// <inheritdoc/>
        public void Record()
        {
            LastActivity = DateTime.UtcNow;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            LastActivity = null;
        }
    }
}