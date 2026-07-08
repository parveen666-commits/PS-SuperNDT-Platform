using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for validating navigation requests.
    /// </summary>
    public interface INavigationValidator
    {
        /// <summary>
        /// Validates a navigation request.
        /// </summary>
        /// <param name="request">The navigation request.</param>
        /// <returns>True if navigation is valid; otherwise false.</returns>
        bool Validate(NavigationRequest request);
    }
}