using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="IRegionNavigationService"/>.
    /// </summary>
    public sealed class RegionNavigationService : IRegionNavigationService
    {
        private readonly IViewService _viewService;
        private readonly INavigationService _navigationService;

        public RegionNavigationService(
            IViewService viewService,
            INavigationService navigationService)
        {
            _viewService = viewService ?? throw new ArgumentNullException(nameof(viewService));
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }

        /// <inheritdoc/>
        public object? CurrentView { get; private set; }

        /// <inheritdoc/>
        public object? CurrentViewModel => _navigationService.CurrentViewModel;

        /// <inheritdoc/>
        public void Show<TViewModel>()
            where TViewModel : class
        {
            Show<TViewModel>(null);
        }

        /// <inheritdoc/>
        public void Show<TViewModel>(object? parameter)
            where TViewModel : class
        {
            _navigationService.NavigateTo<TViewModel>();

            CurrentView = _viewService.CreateView<TViewModel>();
        }
    }
}