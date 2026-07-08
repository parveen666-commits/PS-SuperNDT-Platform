using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service capable of resolving a View from a ViewModel.
    /// </summary>
    public interface IViewService
    {
        /// <summary>
        /// Creates or resolves the View associated with the specified ViewModel type.
        /// </summary>
        /// <param name="viewModelType">The ViewModel type.</param>
        /// <returns>The View instance.</returns>
        object CreateView(Type viewModelType);

        /// <summary>
        /// Creates or resolves the View associated with the specified ViewModel.
        /// </summary>
        /// <typeparam name="TViewModel">The ViewModel type.</typeparam>
        /// <returns>The View instance.</returns>
        object CreateView<TViewModel>() where TViewModel : class;
    }
}