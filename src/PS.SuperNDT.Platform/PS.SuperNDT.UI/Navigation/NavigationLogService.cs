using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationLogService"/>.
    /// </summary>
    public sealed class NavigationLogService : INavigationLogService
    {
        /// <inheritdoc/>
        public string? LastEntry { get; private set; }

        /// <inheritdoc/>
        public void Write(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException(
                    "Log message cannot be null or empty.",
                    nameof(message));
            }

            LastEntry = message;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            LastEntry = null;
        }
    }
}