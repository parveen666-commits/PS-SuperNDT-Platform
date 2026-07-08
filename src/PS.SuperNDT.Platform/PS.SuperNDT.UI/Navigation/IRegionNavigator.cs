using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for navigating inside a specific region.
    /// </summary>
    public interface IRegionNavigator
    {
        /// <summary>
        /// Navigates a region to the specified ViewModel.
        /// </summary>
        /// <typeparam name="TViewModel">Target ViewModel type.</typeparam>
        /// <param name="regionName">Target region name.</param>
        void Navigate<TViewModel>(string regionName)
            where TViewModel : class;

        /// <summary>
        /// Navigates a region to the specified ViewModel with a parameter.
        /// </summary>
        /// <typeparam name="TViewModel">Target ViewModel type.</typeparam>
        /// <param name="regionName">Target region name.</param>
        /// <param name="parameter">Navigation parameter.</param>
        void Navigate<TViewModel>(string regionName, object? parameter)
            where TViewModel : class;
    }
}