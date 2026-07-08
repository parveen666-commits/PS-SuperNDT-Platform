using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationStateObserver"/>.
    /// </summary>
    public sealed class NavigationStateObserver : INavigationStateObserver
    {
        /// <inheritdoc/>
        public event EventHandler? StateChanged;

        /// <inheritdoc/>
        public void NotifyStateChanged()
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}