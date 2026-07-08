using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="IViewService"/>.
    /// </summary>
    public sealed class ViewService : IViewService
    {
        private readonly IViewLocator _viewLocator;

        public ViewService(IViewLocator viewLocator)
        {
            _viewLocator = viewLocator ?? throw new ArgumentNullException(nameof(viewLocator));
        }

        /// <inheritdoc/>
        public object CreateView(Type viewModelType)
        {
            ArgumentNullException.ThrowIfNull(viewModelType);

            var viewType = _viewLocator.GetViewType(viewModelType);

            if (viewType is null)
            {
                throw new InvalidOperationException(
                    $"No view is registered for ViewModel '{viewModelType.FullName}'.");
            }

            var view = Activator.CreateInstance(viewType);

            if (view is null)
            {
                throw new InvalidOperationException(
                    $"Unable to create view '{viewType.FullName}'.");
            }

            return view;
        }

        /// <inheritdoc/>
        public object CreateView<TViewModel>() where TViewModel : class
        {
            return CreateView(typeof(TViewModel));
        }
    }
}