using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationManager"/>.
    /// </summary>
    public sealed class NavigationManagerService : INavigationManager
    {
        private readonly INavigationService _navigationService;
        private readonly INavigationRegistry _navigationRegistry;

        public NavigationManagerService(
            INavigationService navigationService,
            INavigationRegistry navigationRegistry)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            _navigationRegistry = navigationRegistry ?? throw new ArgumentNullException(nameof(navigationRegistry));
        }

        /// <inheritdoc/>
        public object? CurrentViewModel => _navigationService.CurrentViewModel;

        /// <inheritdoc/>
        public void Navigate<TViewModel>()
            where TViewModel : class
        {
            _navigationService.NavigateTo<TViewModel>();
        }

        /// <inheritdoc/>
        public void Navigate<TViewModel>(object? parameter)
            where TViewModel : class
        {
            if (_navigationService is IParameterNavigationService parameterService)
            {
                parameterService.NavigateTo<TViewModel>(parameter);
            }
            else
            {
                _navigationService.NavigateTo<TViewModel>();
            }
        }

        /// <inheritdoc/>
        public void Register<TViewModel, TView>()
            where TViewModel : class
            where TView : class
        {
            _navigationRegistry.Register<TViewModel, TView>();
        }
    }
}