using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Event arguments raised when a navigation request is issued.
    /// </summary>
    public sealed class NavigationRequestedEventArgs : EventArgs
    {
        public NavigationRequestedEventArgs(NavigationRequest request)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
        }

        /// <summary>
        /// Gets the navigation request.
        /// </summary>
        public NavigationRequest Request { get; }
    }
}