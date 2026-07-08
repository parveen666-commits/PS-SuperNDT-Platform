using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for tracking navigation transitions.
    /// </summary>
    public interface INavigationTransitionTracker
    {
        /// <summary>
        /// Gets the last source ViewModel type.
        /// </summary>
        Type? LastSource { get; }

        /// <summary>
        /// Gets the last target ViewModel type.
        /// </summary>
        Type? LastTarget { get; }

        /// <summary>
        /// Records a navigation transition.
        /// </summary>
        /// <param name="source">Source ViewModel type.</param>
        /// <param name="target">Target ViewModel type.</param>
        void Track(Type? source, Type target);

        /// <summary>
        /// Clears transition tracking information.
        /// </summary>
        void Clear();
    }
}