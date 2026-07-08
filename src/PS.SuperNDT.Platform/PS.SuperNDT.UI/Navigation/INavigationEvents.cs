using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a navigation service that raises notifications before and after navigation.
    /// </summary>
    public interface INavigationEvents
    {
        /// <summary>
        /// Occurs before navigation begins.
        /// </summary>
        event EventHandler<NavigationRequest>? Navigating;

        /// <summary>
        /// Occurs after navigation has completed.
        /// </summary>
        event EventHandler<ViewModelNavigatedEventArgs>? Navigated;
    }
}