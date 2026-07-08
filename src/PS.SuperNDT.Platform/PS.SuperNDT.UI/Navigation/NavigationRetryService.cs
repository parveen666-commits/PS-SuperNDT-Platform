using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationRetryService"/>.
    /// </summary>
    public sealed class NavigationRetryService : INavigationRetryService
    {
        public NavigationRetryService(int maxRetries = 3)
        {
            if (maxRetries <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxRetries),
                    "Maximum retry count must be greater than zero.");
            }

            MaxRetries = maxRetries;
        }

        /// <inheritdoc/>
        public int MaxRetries { get; }

        /// <inheritdoc/>
        public int RetryCount { get; private set; }

        /// <inheritdoc/>
        public bool CanRetry => RetryCount < MaxRetries;

        /// <inheritdoc/>
        public void Increment()
        {
            if (CanRetry)
            {
                RetryCount++;
            }
        }

        /// <inheritdoc/>
        public void Reset()
        {
            RetryCount = 0;
        }
    }
}