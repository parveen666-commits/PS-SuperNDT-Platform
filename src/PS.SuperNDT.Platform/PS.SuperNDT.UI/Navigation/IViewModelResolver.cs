using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Provides a resolver for ViewModel instances.
    /// </summary>
    public interface IViewModelResolver
    {
        /// <summary>
        /// Resolves a ViewModel instance for the specified type.
        /// </summary>
        /// <param name="viewModelType">The ViewModel type.</param>
        /// <returns>The resolved ViewModel instance.</returns>
        object Resolve(Type viewModelType);

        /// <summary>
        /// Resolves a ViewModel instance for the specified generic type.
        /// </summary>
        /// <typeparam name="TViewModel">The ViewModel type.</typeparam>
        /// <returns>The resolved ViewModel instance.</returns>
        TViewModel Resolve<TViewModel>() where TViewModel : class;
    }
}