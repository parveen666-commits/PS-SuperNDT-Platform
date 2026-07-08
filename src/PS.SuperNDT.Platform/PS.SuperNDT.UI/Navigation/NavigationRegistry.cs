using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationRegistry"/>.
    /// </summary>
    public sealed class NavigationRegistry : INavigationRegistry
    {
        private readonly IViewLocator _viewLocator;

        public NavigationRegistry(IViewLocator viewLocator)
        {
            _viewLocator = viewLocator ?? throw new ArgumentNullException(nameof(viewLocator));
        }

        /// <inheritdoc/>
        public void Register<TViewModel, TView>()
            where TViewModel : class
            where TView : class
        {
            _viewLocator.Register<TViewModel, TView>();
        }

        /// <inheritdoc/>
        public bool IsRegistered(Type viewModelType)
        {
            ArgumentNullException.ThrowIfNull(viewModelType);

            return _viewLocator.GetViewType(viewModelType) != null;
        }
    }
}