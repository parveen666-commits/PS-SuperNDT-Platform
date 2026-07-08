using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service that manages the active navigation view.
    /// </summary>
    public interface IRegionNavigationService
    {
        /// <summary>
        /// Gets the currently displayed view.
        /// </summary>
        object? CurrentView { get; }

        /// <summary>
        /// Gets the currently displayed ViewModel.
        /// </summary>
        object? CurrentViewModel { get; }

        /// <summary>
        /// Displays a view for the specified ViewModel.
        /// </summary>
        /// <typeparam name="TViewModel">The ViewModel type.</typeparam>
        void Show<TViewModel>()
            where TViewModel : class;

        /// <summary>
        /// Displays a view for the specified ViewModel with a parameter.
        /// </summary>
        /// <typeparam name="TViewModel">The ViewModel type.</typeparam>
        /// <param name="parameter">Navigation parameter.</param>
        void Show<TViewModel>(object? parameter)
            where TViewModel : class;
    }
}