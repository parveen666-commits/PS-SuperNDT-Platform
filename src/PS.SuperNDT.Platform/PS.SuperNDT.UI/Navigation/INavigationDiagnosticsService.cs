using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for navigation diagnostics.
    /// </summary>
    public interface INavigationDiagnosticsService
    {
        /// <summary>
        /// Gets the latest diagnostic message.
        /// </summary>
        string? LastMessage { get; }

        /// <summary>
        /// Records a diagnostic message.
        /// </summary>
        /// <param name="message">Diagnostic message.</param>
        void Record(string message);

        /// <summary>
        /// Clears diagnostics.
        /// </summary>
        void Clear();
    }
}