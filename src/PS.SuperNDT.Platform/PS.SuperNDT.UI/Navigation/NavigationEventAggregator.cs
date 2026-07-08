using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationEventAggregator"/>.
    /// </summary>
    public sealed class NavigationEventAggregator : INavigationEventAggregator
    {
        /// <inheritdoc/>
        public event EventHandler<NavigationRequest>? NavigationStarted;

        /// <inheritdoc/>
        public event EventHandler<ViewModelNavigatedEventArgs>? NavigationCompleted;

        /// <inheritdoc/>
        public void PublishNavigationStarted(NavigationRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            NavigationStarted?.Invoke(this, request);
        }

        /// <inheritdoc/>
        public void PublishNavigationCompleted(ViewModelNavigatedEventArgs args)
        {
            ArgumentNullException.ThrowIfNull(args);

            NavigationCompleted?.Invoke(this, args);
        }
    }
}