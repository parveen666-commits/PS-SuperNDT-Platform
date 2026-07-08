using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for navigation request validation results.
    /// </summary>
    public interface INavigationResultService
    {
        /// <summary>
        /// Gets the last navigation result.
        /// </summary>
        NavigationResult? LastResult { get; }

        /// <summary>
        /// Stores a navigation result.
        /// </summary>
        /// <param name="result">Navigation result.</param>
        void SetResult(NavigationResult result);

        /// <summary>
        /// Clears the stored navigation result.
        /// </summary>
        void Clear();
    }
}