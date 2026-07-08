using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="IRegionNavigator"/>.
    /// </summary>
    public sealed class RegionNavigator : IRegionNavigator
    {
        private readonly IRegionManager _regionManager;
        private readonly IViewService _viewService;
        private readonly IViewModelProvider _viewModelProvider;

        public RegionNavigator(
            IRegionManager regionManager,
            IViewService viewService,
            IViewModelProvider viewModelProvider)
        {
            _regionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));
            _viewService = viewService ?? throw new ArgumentNullException(nameof(viewService));
            _viewModelProvider = viewModelProvider ?? throw new ArgumentNullException(nameof(viewModelProvider));
        }

        /// <inheritdoc/>
        public void Navigate<TViewModel>(string regionName)
            where TViewModel : class
        {
            Navigate<TViewModel>(regionName, null);
        }

        /// <inheritdoc/>
        public void Navigate<TViewModel>(string regionName, object? parameter)
            where TViewModel : class
        {
            var region = _regionManager.GetRegion(regionName);

            if (region is null)
            {
                throw new InvalidOperationException(
                    $"Navigation region '{regionName}' was not found.");
            }

            var view = _viewService.CreateView<TViewModel>();
            var viewModel = _viewModelProvider.Get<TViewModel>();

            region.SetContent(view, viewModel);
        }
    }
}