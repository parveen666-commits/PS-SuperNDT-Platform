using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationStatisticsService"/>.
    /// </summary>
    public sealed class NavigationStatisticsService : INavigationStatisticsService
    {
        /// <inheritdoc/>
        public int TotalNavigations { get; private set; }

        /// <inheritdoc/>
        public int SuccessfulNavigations { get; private set; }

        /// <inheritdoc/>
        public int FailedNavigations { get; private set; }

        /// <inheritdoc/>
        public void RecordSuccess()
        {
            TotalNavigations++;
            SuccessfulNavigations++;
        }

        /// <inheritdoc/>
        public void RecordFailure()
        {
            TotalNavigations++;
            FailedNavigations++;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            TotalNavigations = 0;
            SuccessfulNavigations = 0;
            FailedNavigations = 0;
        }
    }
}