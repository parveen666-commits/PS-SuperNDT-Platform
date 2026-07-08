using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationUserActivityService"/>.
    /// </summary>
    public sealed class NavigationUserActivityService : INavigationUserActivityService
    {
        /// <inheritdoc/>
        public DateTime? LastActivityTime { get; private set; }

        /// <inheritdoc/>
        public void RecordActivity()
        {
            LastActivityTime = DateTime.UtcNow;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            LastActivityTime = null;
        }
    }
}