using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationStateValidator"/>.
    /// </summary>
    public sealed class NavigationStateValidator : INavigationStateValidator
    {
        /// <inheritdoc/>
        public bool Validate(INavigationState state)
        {
            ArgumentNullException.ThrowIfNull(state);

            return state.CurrentViewModel != null;
        }
    }
}