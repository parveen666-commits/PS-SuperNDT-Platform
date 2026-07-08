using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationCommands"/>.
    /// </summary>
    public sealed class NavigationCommands : INavigationCommands
    {
        private readonly INavigationService _navigationService;

        public NavigationCommands(INavigationService navigationService)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }

        /// <inheritdoc/>
        public bool CanGoBack => _navigationService.CanGoBack;

        /// <inheritdoc/>
        public void Navigate<TViewModel>()
            where TViewModel : class
        {
            _navigationService.NavigateTo<TViewModel>();
        }

        /// <inheritdoc/>
        public void GoBack()
        {
            _navigationService.GoBack();
        }

        /// <inheritdoc/>
        public void ClearHistory()
        {
            _navigationService.ClearHistory();
        }
    }
}