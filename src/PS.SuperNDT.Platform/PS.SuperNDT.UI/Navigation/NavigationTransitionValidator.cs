using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationTransitionValidator"/>.
    /// </summary>
    public sealed class NavigationTransitionValidator : INavigationTransitionValidator
    {
        /// <inheritdoc/>
        public bool CanTransition(Type? from, Type to)
        {
            ArgumentNullException.ThrowIfNull(to);

            // Allows navigation when there is no current ViewModel
            // or when the target ViewModel is different.
            return from == null || from != to;
        }
    }
}