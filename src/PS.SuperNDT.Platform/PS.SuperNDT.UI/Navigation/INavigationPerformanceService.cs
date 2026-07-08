using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for monitoring navigation performance.
    /// </summary>
    public interface INavigationPerformanceService
    {
        /// <summary>
        /// Gets the last navigation duration.
        /// </summary>
        TimeSpan? LastNavigationDuration { get; }

        /// <summary>
        /// Starts performance measurement.
        /// </summary>
        void StartMeasurement();

        /// <summary>
        /// Stops performance measurement.
        /// </summary>
        void StopMeasurement();
    }
}