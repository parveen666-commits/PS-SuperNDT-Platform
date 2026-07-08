using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for navigation diagnostics logging.
    /// </summary>
    public interface INavigationLogService
    {
        /// <summary>
        /// Gets the latest navigation log entry.
        /// </summary>
        string? LastEntry { get; }

        /// <summary>
        /// Writes a navigation log entry.
        /// </summary>
        /// <param name="message">Log message.</param>
        void Write(string message);

        /// <summary>
        /// Clears navigation logs.
        /// </summary>
        void Clear();
    }
}