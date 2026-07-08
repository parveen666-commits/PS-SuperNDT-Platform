using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Provides a mechanism for ViewModels to request navigation
    /// without directly depending on the navigation service.
    /// </summary>
    public interface INavigationProvider
    {
        /// <summary>
        /// Raised when a navigation request is made.
        /// </summary>
        event EventHandler<NavigationRequest>? NavigationRequested;
    }
}