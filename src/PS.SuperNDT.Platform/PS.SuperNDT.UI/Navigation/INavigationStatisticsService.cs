using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for tracking navigation statistics.
    /// </summary>
    public interface INavigationStatisticsService
    {
        /// <summary>
        /// Gets the total number of navigation attempts.
        /// </summary>
        int TotalNavigations { get; }

        /// <summary>
        /// Gets the number of successful navigations.
        /// </summary>
        int SuccessfulNavigations { get; }

        /// <summary>
        /// Gets the number of failed navigations.
        /// </summary>
        int FailedNavigations { get; }

        /// <summary>
        /// Records a successful navigation.
        /// </summary>
        void RecordSuccess();

        /// <summary>
        /// Records a failed navigation.
        /// </summary>
        void RecordFailure();

        /// <summary>
        /// Resets all statistics.
        /// </summary>
        void Reset();
    }
}