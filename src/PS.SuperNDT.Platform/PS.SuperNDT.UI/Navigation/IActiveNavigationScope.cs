using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for storing the active navigation scope.
    /// </summary>
    public interface IActiveNavigationScope
    {
        /// <summary>
        /// Gets the active navigation scope.
        /// </summary>
        INavigationScope? CurrentScope { get; }

        /// <summary>
        /// Sets the active navigation scope.
        /// </summary>
        /// <param name="scope">The navigation scope.</param>
        void SetScope(INavigationScope scope);

        /// <summary>
        /// Clears the active navigation scope.
        /// </summary>
        void Clear();
    }
}