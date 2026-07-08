using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service that manages navigation targets and registrations.
    /// </summary>
    public interface INavigationManager
    {
        /// <summary>
        /// Gets the current ViewModel.
        /// </summary>
        object? CurrentViewModel { get; }

        /// <summary>
        /// Navigates to the specified ViewModel.
        /// </summary>
        /// <typeparam name="TViewModel">Target ViewModel type.</typeparam>
        void Navigate<TViewModel>()
            where TViewModel : class;

        /// <summary>
        /// Navigates to the specified ViewModel with a parameter.
        /// </summary>
        /// <typeparam name="TViewModel">Target ViewModel type.</typeparam>
        /// <param name="parameter">Navigation parameter.</param>
        void Navigate<TViewModel>(object? parameter)
            where TViewModel : class;

        /// <summary>
        /// Registers a ViewModel and View mapping.
        /// </summary>
        /// <typeparam name="TViewModel">ViewModel type.</typeparam>
        /// <typeparam name="TView">View type.</typeparam>
        void Register<TViewModel, TView>()
            where TViewModel : class
            where TView : class;
    }
}