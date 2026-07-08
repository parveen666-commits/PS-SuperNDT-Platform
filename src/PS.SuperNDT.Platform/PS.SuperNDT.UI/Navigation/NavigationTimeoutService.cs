using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationTimeoutService"/>.
    /// </summary>
    public sealed class NavigationTimeoutService : INavigationTimeoutService
    {
        /// <inheritdoc/>
        public TimeSpan Timeout { get; private set; } = TimeSpan.FromSeconds(30);

        /// <inheritdoc/>
        public bool IsExpired(DateTime startTime)
        {
            return DateTime.UtcNow - startTime >= Timeout;
        }

        /// <inheritdoc/>
        public void SetTimeout(TimeSpan timeout)
        {
            if (timeout <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(timeout),
                    "Timeout must be greater than zero.");
            }

            Timeout = timeout;
        }
    }
}