using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for observing navigation state updates.
    /// </summary>
    public interface INavigationStateObserver
    {
        /// <summary>
        /// Occurs when the navigation state changes.
        /// </summary>
        event EventHandler? StateChanged;

        /// <summary>
        /// Raises the navigation state changed event.
        /// </summary>
        void NotifyStateChanged();
    }
}