using System;
using System.Collections.Generic;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="IViewLocator"/>.
    /// </summary>
    public sealed class ViewLocator : IViewLocator
    {
        private readonly Dictionary<Type, Type> _mappings = new();

        /// <inheritdoc/>
        public void Register<TViewModel, TView>()
            where TViewModel : class
            where TView : class
        {
            _mappings[typeof(TViewModel)] = typeof(TView);
        }

        /// <inheritdoc/>
        public Type? GetViewType(Type viewModelType)
        {
            ArgumentNullException.ThrowIfNull(viewModelType);

            return _mappings.TryGetValue(viewModelType, out var viewType)
                ? viewType
                : null;
        }
    }
}