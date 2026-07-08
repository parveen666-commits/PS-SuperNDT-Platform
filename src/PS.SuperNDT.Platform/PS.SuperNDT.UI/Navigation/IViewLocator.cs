using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Maps ViewModel types to their corresponding View types.
    /// </summary>
    public interface IViewLocator
    {
        /// <summary>
        /// Registers a View for a ViewModel.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TView">View type.</typeparam>
        void Register<TViewModel, TView>()
            where TViewModel : class
            where TView : class;

        /// <summary>
        /// Gets the View type associated with the specified ViewModel type.
        /// </summary>
        /// <param name="viewModelType">The ViewModel type.</param>
        /// <returns>The View type, or null if not registered.</returns>
        Type? GetViewType(Type viewModelType);
    }
}