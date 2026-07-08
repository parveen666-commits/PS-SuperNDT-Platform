using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for controlling navigation history limits.
    /// </summary>
    public interface INavigationHistoryPolicy
    {
        /// <summary>
        /// Gets the maximum number of history entries allowed.
        /// </summary>
        int MaximumEntries { get; }

        /// <summary>
        /// Determines whether a new entry can be added.
        /// </summary>
        /// <returns>True if allowed; otherwise false.</returns>
        bool CanAddEntry();

        /// <summary>
        /// Removes old entries according to the policy.
        /// </summary>
        void Trim();
    }
}