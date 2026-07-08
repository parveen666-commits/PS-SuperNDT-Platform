using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines navigation lifecycle events exposed to the UI layer.
    /// </summary>
    public interface INavigationEventAggregator
    {
        /// <summary>
        /// Occurs when navigation starts.
        /// </summary>
        event EventHandler<NavigationRequest>? NavigationStarted;

        /// <summary>
        /// Occurs when navigation completes.
        /// </summary>
        event EventHandler<ViewModelNavigatedEventArgs>? NavigationCompleted;

        /// <summary>
        /// Publishes a navigation started event.
        /// </summary>
        /// <param name="request">Navigation request information.</param>
        void PublishNavigationStarted(NavigationRequest request);

        /// <summary>
        /// Publishes a navigation completed event.
        /// </summary>
        /// <param name="args">Navigation completion information.</param>
        void PublishNavigationCompleted(ViewModelNavigatedEventArgs args);
    }
}