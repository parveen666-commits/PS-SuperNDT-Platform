using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for handling navigation redirects.
    /// </summary>
    public interface INavigationRedirectService
    {
        /// <summary>
        /// Adds a navigation redirect rule.
        /// </summary>
        /// <param name="source">Source ViewModel type.</param>
        /// <param name="target">Target ViewModel type.</param>
        void AddRedirect(Type source, Type target);

        /// <summary>
        /// Resolves a redirected navigation target.
        /// </summary>
        /// <param name="source">Source ViewModel type.</param>
        /// <returns>Redirect target type if available.</returns>
        Type? Resolve(Type source);
    }
}