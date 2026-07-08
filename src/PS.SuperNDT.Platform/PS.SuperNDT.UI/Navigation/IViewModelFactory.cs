using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Factory used to create ViewModel instances for navigation.
    /// </summary>
    public interface IViewModelFactory
    {
        /// <summary>
        /// Creates an instance of the specified ViewModel type.
        /// </summary>
        /// <typeparam name="TViewModel">The ViewModel type.</typeparam>
        /// <returns>The created ViewModel instance.</returns>
        TViewModel Create<TViewModel>() where TViewModel : class;

        /// <summary>
        /// Creates an instance of the specified ViewModel type.
        /// </summary>
        /// <param name="viewModelType">The ViewModel type.</param>
        /// <returns>The created ViewModel instance.</returns>
        object Create(Type viewModelType);
    }
}