using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for navigation state validation.
    /// </summary>
    public interface INavigationStateValidator
    {
        /// <summary>
        /// Validates the current navigation state.
        /// </summary>
        /// <param name="state">Navigation state.</param>
        /// <returns>True if valid; otherwise false.</returns>
        bool Validate(INavigationState state);
    }
}