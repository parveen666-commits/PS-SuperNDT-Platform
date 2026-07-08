using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a navigation service that supports parameter-based navigation.
    /// </summary>
    public interface IParameterNavigationService : INavigationService
    {
        /// <summary>
        /// Navigates to the specified ViewModel and passes an optional parameter.
        /// </summary>
        /// <typeparam name="TViewModel">Destination ViewModel type.</typeparam>
        /// <param name="parameter">Optional navigation parameter.</param>
        void NavigateTo<TViewModel>(object? parameter) where TViewModel : class;
    }
}