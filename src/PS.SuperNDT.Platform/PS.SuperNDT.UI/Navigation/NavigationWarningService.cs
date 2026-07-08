using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationWarningService"/>.
    /// </summary>
    public sealed class NavigationWarningService : INavigationWarningService
    {
        /// <inheritdoc/>
        public string? LastWarning { get; private set; }

        /// <inheritdoc/>
        public void AddWarning(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException(
                    "Warning message cannot be empty.",
                    nameof(message));
            }

            LastWarning = message;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            LastWarning = null;
        }
    }
}