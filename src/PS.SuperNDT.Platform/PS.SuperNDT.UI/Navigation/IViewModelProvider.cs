using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service that can resolve registered ViewModel instances.
    /// </summary>
    public interface IViewModelProvider
    {
        /// <summary>
        /// Gets a ViewModel instance of the specified type.
        /// </summary>
        /// <typeparam name="TViewModel">The ViewModel type.</typeparam>
        /// <returns>The resolved ViewModel instance.</returns>
        TViewModel Get<TViewModel>() where TViewModel : class;

        /// <summary>
        /// Gets a ViewModel instance by its runtime type.
        /// </summary>
        /// <param name="viewModelType">The ViewModel type.</param>
        /// <returns>The resolved ViewModel instance.</returns>
        object Get(Type viewModelType);
    }
}