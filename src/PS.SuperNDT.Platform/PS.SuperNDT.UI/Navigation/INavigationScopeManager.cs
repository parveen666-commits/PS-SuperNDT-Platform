using System;
using System.Collections.Generic;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a manager for creating and accessing navigation scopes.
    /// </summary>
    public interface INavigationScopeManager
    {
        /// <summary>
        /// Gets all available navigation scopes.
        /// </summary>
        IReadOnlyDictionary<string, INavigationScope> Scopes { get; }

        /// <summary>
        /// Creates a navigation scope.
        /// </summary>
        /// <param name="scope">The navigation scope.</param>
        void Register(INavigationScope scope);

        /// <summary>
        /// Gets a navigation scope by name.
        /// </summary>
        /// <param name="name">Scope name.</param>
        /// <returns>The navigation scope if found.</returns>
        INavigationScope? Get(string name);

        /// <summary>
        /// Removes a navigation scope.
        /// </summary>
        /// <param name="name">Scope name.</param>
        void Remove(string name);
    }
}