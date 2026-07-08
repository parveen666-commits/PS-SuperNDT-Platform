using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for registering ViewModel-to-View mappings.
    /// </summary>
    public interface INavigationRegistry
    {
        /// <summary>
        /// Registers a ViewModel and its corresponding View.
        /// </summary>
        /// <typeparam name="TViewModel">The ViewModel type.</typeparam>
        /// <typeparam name="TView">The View type.</typeparam>
        void Register<TViewModel, TView>()
            where TViewModel : class
            where TView : class;

        /// <summary>
        /// Determines whether a ViewModel has been registered.
        /// </summary>
        /// <param name="viewModelType">The ViewModel type.</param>
        /// <returns><c>true</c> if the ViewModel is registered; otherwise <c>false</c>.</returns>
        bool IsRegistered(Type viewModelType);
    }
}