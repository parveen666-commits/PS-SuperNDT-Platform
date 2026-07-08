using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationHistoryMonitor"/>.
    /// </summary>
    public sealed class NavigationHistoryMonitor : INavigationHistoryMonitor
    {
        /// <inheritdoc/>
        public int HistoryCount { get; private set; }

        /// <inheritdoc/>
        public void Update(int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(count),
                    "History count cannot be negative.");
            }

            HistoryCount = count;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            HistoryCount = 0;
        }
    }
}