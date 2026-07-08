using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="IViewProvider"/>.
    /// </summary>
    public sealed class ViewProvider : IViewProvider
    {
        private readonly IViewService _viewService;

        public ViewProvider(IViewService viewService)
        {
            _viewService = viewService ?? throw new ArgumentNullException(nameof(viewService));
        }

        /// <inheritdoc/>
        public object GetView<TViewModel>() where TViewModel : class
        {
            return _viewService.CreateView<TViewModel>();
        }

        /// <inheritdoc/>
        public object GetView(Type viewModelType)
        {
            ArgumentNullException.ThrowIfNull(viewModelType);

            return _viewService.CreateView(viewModelType);
        }
    }
}