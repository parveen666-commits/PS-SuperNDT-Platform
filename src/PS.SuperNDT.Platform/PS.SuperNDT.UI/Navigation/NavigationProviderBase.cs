using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Base class for ViewModels that can request navigation.
    /// </summary>
    public abstract class NavigationProviderBase : INavigationProvider
    {
        /// <inheritdoc/>
        public event EventHandler<NavigationRequest>? NavigationRequested;

        /// <summary>
        /// Raises a navigation request for the specified ViewModel.
        /// </summary>
        /// <typeparam name="TViewModel">Destination ViewModel type.</typeparam>
        /// <param name="parameter">Optional navigation parameter.</param>
        protected void RequestNavigation<TViewModel>(object? parameter = null)
            where TViewModel : class
        {
            NavigationRequested?.Invoke(
                this,
                new NavigationRequest(typeof(TViewModel), parameter));
        }
    }
}