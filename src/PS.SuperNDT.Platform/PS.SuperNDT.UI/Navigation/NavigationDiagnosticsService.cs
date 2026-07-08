using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationDiagnosticsService"/>.
    /// </summary>
    public sealed class NavigationDiagnosticsService : INavigationDiagnosticsService
    {
        /// <inheritdoc/>
        public string? LastMessage { get; private set; }

        /// <inheritdoc/>
        public void Record(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException(
                    "Diagnostic message cannot be empty.",
                    nameof(message));
            }

            LastMessage = message;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            LastMessage = null;
        }
    }
}