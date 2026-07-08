using System;
using System.Diagnostics;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationPerformanceService"/>.
    /// </summary>
    public sealed class NavigationPerformanceService : INavigationPerformanceService
    {
        private readonly Stopwatch _stopwatch = new();

        /// <inheritdoc/>
        public TimeSpan? LastNavigationDuration { get; private set; }

        /// <inheritdoc/>
        public void StartMeasurement()
        {
            _stopwatch.Restart();
        }

        /// <inheritdoc/>
        public void StopMeasurement()
        {
            if (_stopwatch.IsRunning)
            {
                _stopwatch.Stop();
                LastNavigationDuration = _stopwatch.Elapsed;
            }
        }
    }
}