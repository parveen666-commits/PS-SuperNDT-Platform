using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a ViewModel that can receive a navigation parameter.
    /// </summary>
    public interface INavigationAware
    {
        /// <summary>
        /// Called after navigation to this ViewModel.
        /// </summary>
        /// <param name="parameter">Optional navigation parameter.</param>
        void OnNavigatedTo(object? parameter);

        /// <summary>
        /// Called before navigating away from this ViewModel.
        /// </summary>
        void OnNavigatedFrom();
    }
}