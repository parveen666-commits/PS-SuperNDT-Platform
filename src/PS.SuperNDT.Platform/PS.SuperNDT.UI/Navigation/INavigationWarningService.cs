using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for tracking navigation warnings.
    /// </summary>
    public interface INavigationWarningService
    {
        /// <summary>
        /// Gets the latest warning message.
        /// </summary>
        string? LastWarning { get; }

        /// <summary>
        /// Adds a navigation warning.
        /// </summary>
        /// <param name="message">Warning message.</param>
        void AddWarning(string message);

        /// <summary>
        /// Clears the current warning.
        /// </summary>
        void Clear();
    }
}