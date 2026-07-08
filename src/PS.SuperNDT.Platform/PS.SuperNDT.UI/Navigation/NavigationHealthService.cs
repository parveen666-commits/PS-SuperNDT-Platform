using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationHealthService"/>.
    /// </summary>
    public sealed class NavigationHealthService : INavigationHealthService
    {
        /// <inheritdoc/>
        public bool IsHealthy { get; private set; }

        /// <inheritdoc/>
        public string? StatusMessage { get; private set; }

        /// <inheritdoc/>
        public void Update(bool healthy, string? message = null)
        {
            IsHealthy = healthy;
            StatusMessage = message;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            IsHealthy = false;
            StatusMessage = null;
        }
    }
}