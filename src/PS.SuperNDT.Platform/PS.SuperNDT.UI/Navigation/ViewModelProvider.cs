using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="IViewModelProvider"/>.
    /// </summary>
    public sealed class ViewModelProvider : IViewModelProvider
    {
        private readonly IViewModelFactory _viewModelFactory;

        public ViewModelProvider(IViewModelFactory viewModelFactory)
        {
            _viewModelFactory = viewModelFactory ?? throw new ArgumentNullException(nameof(viewModelFactory));
        }

        /// <inheritdoc/>
        public TViewModel Get<TViewModel>() where TViewModel : class
        {
            return _viewModelFactory.Create<TViewModel>();
        }

        /// <inheritdoc/>
        public object Get(Type viewModelType)
        {
            ArgumentNullException.ThrowIfNull(viewModelType);

            return _viewModelFactory.Create(viewModelType);
        }
    }
}