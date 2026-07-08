using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="IViewModelFactory"/>.
    /// Creates ViewModels using their parameterless constructor.
    /// </summary>
    public sealed class ViewModelFactory : IViewModelFactory
    {
        /// <inheritdoc/>
        public TViewModel Create<TViewModel>() where TViewModel : class
        {
            return (TViewModel)Create(typeof(TViewModel));
        }

        /// <inheritdoc/>
        public object Create(Type viewModelType)
        {
            ArgumentNullException.ThrowIfNull(viewModelType);

            var instance = Activator.CreateInstance(viewModelType);

            if (instance is null)
            {
                throw new InvalidOperationException(
                    $"Unable to create an instance of '{viewModelType.FullName}'.");
            }

            return instance;
        }
    }
}