using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for creating and managing navigation scopes.
    /// </summary>
    public interface INavigationScope
    {
        /// <summary>
        /// Gets the unique scope name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the navigation context associated with this scope.
        /// </summary>
        INavigationContext Context { get; }
    }
}