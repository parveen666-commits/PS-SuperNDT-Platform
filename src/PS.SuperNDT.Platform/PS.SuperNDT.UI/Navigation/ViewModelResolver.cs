using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="IViewModelResolver"/>.
    /// Uses an <see cref="IViewModelFactory"/> to create ViewModel instances.
    /// </summary>
    public sealed class ViewModelResolver : IViewModelResolver
    {
        private readonly IViewModelFactory _factory;

        public ViewModelResolver(IViewModelFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        /// <inheritdoc/>
        public object Resolve(Type viewModelType)
        {
            return _factory.Create(viewModelType);
        }

        /// <inheritdoc/>
        public TViewModel Resolve<TViewModel>() where TViewModel : class
        {
            return _factory.Create<TViewModel>();
        }
    }
}