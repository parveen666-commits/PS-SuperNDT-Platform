using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for validating ViewModel navigation transitions.
    /// </summary>
    public interface INavigationTransitionValidator
    {
        /// <summary>
        /// Determines whether transition from one ViewModel to another is allowed.
        /// </summary>
        /// <param name="from">Current ViewModel type.</param>
        /// <param name="to">Target ViewModel type.</param>
        /// <returns>True if transition is allowed; otherwise false.</returns>
        bool CanTransition(Type? from, Type to);
    }
}