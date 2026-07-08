using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service responsible for coordinating navigation operations.
    /// </summary>
    public interface INavigationCoordinator
    {
        /// <summary>
        /// Gets the current navigation state.
        /// </summary>
        INavigationState State { get; }

        /// <summary>
        /// Navigates to the specified ViewModel type.
        /// </summary>
        /// <typeparam name="TViewModel">Target ViewModel type.</typeparam>
        void Navigate<TViewModel>() where TViewModel : class;

        /// <summary>
        /// Navigates to the specified ViewModel type with a parameter.
        /// </summary>
        /// <typeparam name="TViewModel">Target ViewModel type.</typeparam>
        /// <param name="parameter">Navigation parameter.</param>
        void Navigate<TViewModel>(object? parameter)
            where TViewModel : class;
    }
}