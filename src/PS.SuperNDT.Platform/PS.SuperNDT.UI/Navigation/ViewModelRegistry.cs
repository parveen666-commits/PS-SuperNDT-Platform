using System;
using System.Collections.Generic;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="IViewModelRegistry"/>.
    /// </summary>
    public sealed class ViewModelRegistry : IViewModelRegistry
    {
        private readonly Dictionary<Type, Func<object>> _factories = new();

        /// <inheritdoc/>
        public void Register<TViewModel>(Func<TViewModel> factory)
            where TViewModel : class
        {
            ArgumentNullException.ThrowIfNull(factory);

            _factories[typeof(TViewModel)] = () => factory();
        }

        /// <inheritdoc/>
        public bool IsRegistered<TViewModel>()
            where TViewModel : class
        {
            return _factories.ContainsKey(typeof(TViewModel));
        }

        /// <summary>
        /// Creates a registered ViewModel instance.
        /// </summary>
        /// <typeparam name="TViewModel">The ViewModel type.</typeparam>
        /// <returns>The created ViewModel.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the ViewModel type has not been registered.
        /// </exception>
        public TViewModel Create<TViewModel>()
            where TViewModel : class
        {
            if (!_factories.TryGetValue(typeof(TViewModel), out var factory))
            {
                throw new InvalidOperationException(
                    $"ViewModel '{typeof(TViewModel).FullName}' is not registered.");
            }

            return (TViewModel)factory();
        }
    }
}