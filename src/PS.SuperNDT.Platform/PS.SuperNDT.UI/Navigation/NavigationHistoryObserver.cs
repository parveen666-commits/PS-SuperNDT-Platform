using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationHistoryObserver"/>.
    /// </summary>
    public sealed class NavigationHistoryObserver : INavigationHistoryObserver
    {
        /// <inheritdoc/>
        public event EventHandler? HistoryChanged;

        /// <inheritdoc/>
        public void NotifyHistoryChanged()
        {
            HistoryChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}