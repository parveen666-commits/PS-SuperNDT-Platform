using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationStateService"/>.
    /// </summary>
    public sealed class NavigationStateService : INavigationStateService
    {
        public NavigationStateService(INavigationState state)
        {
            State = state ?? throw new ArgumentNullException(nameof(state));
        }

        /// <inheritdoc/>
        public INavigationState State { get; }

        /// <inheritdoc/>
        public void UpdateState(object? viewModel)
        {
            if (State is NavigationState navigationState)
            {
                navigationState.Update(viewModel);
            }
        }

        /// <inheritdoc/>
        public void Reset()
        {
            if (State is NavigationState navigationState)
            {
                navigationState.Reset();
            }
        }
    }
}