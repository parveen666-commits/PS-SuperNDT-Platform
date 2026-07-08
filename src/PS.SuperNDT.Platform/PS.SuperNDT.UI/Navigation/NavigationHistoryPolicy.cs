using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationHistoryPolicy"/>.
    /// </summary>
    public sealed class NavigationHistoryPolicy : INavigationHistoryPolicy
    {
        public NavigationHistoryPolicy(int maximumEntries = 50)
        {
            if (maximumEntries <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maximumEntries),
                    "Maximum entries must be greater than zero.");
            }

            MaximumEntries = maximumEntries;
        }

        /// <inheritdoc/>
        public int MaximumEntries { get; }

        /// <inheritdoc/>
        public bool CanAddEntry()
        {
            return true;
        }

        /// <inheritdoc/>
        public void Trim()
        {
            // History trimming is handled by the navigation history implementation.
        }
    }
}