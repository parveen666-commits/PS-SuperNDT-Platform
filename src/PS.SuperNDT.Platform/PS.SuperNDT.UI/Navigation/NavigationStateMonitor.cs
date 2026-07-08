using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationStateMonitor"/>.
    /// </summary>
    public sealed class NavigationStateMonitor : INavigationStateMonitor
    {
        /// <inheritdoc/>
        public bool IsValid { get; private set; }

        /// <inheritdoc/>
        public void Update(INavigationState state)
        {
            ArgumentNullException.ThrowIfNull(state);

            IsValid = state.CurrentViewModel != null;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            IsValid = false;
        }
    }
}