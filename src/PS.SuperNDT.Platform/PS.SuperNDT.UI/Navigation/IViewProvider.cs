using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service that resolves registered View instances.
    /// </summary>
    public interface IViewProvider
    {
        /// <summary>
        /// Gets a View instance for the specified ViewModel type.
        /// </summary>
        /// <typeparam name="TViewModel">The ViewModel type.</typeparam>
        /// <returns>The View instance.</returns>
        object GetView<TViewModel>() where TViewModel : class;

        /// <summary>
        /// Gets a View instance for the specified ViewModel type.
        /// </summary>
        /// <param name="viewModelType">The ViewModel type.</param>
        /// <returns>The View instance.</returns>
        object GetView(Type viewModelType);
    }
}