using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for tracking navigation state history changes.
    /// </summary>
    public interface INavigationHistoryObserver
    {
        /// <summary>
        /// Occurs when navigation history changes.
        /// </summary>
        event EventHandler? HistoryChanged;

        /// <summary>
        /// Notifies that navigation history has changed.
        /// </summary>
        void NotifyHistoryChanged();
    }
}