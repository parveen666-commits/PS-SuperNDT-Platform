using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Provides a central registry for navigation-related services.
    /// </summary>
    public interface INavigationContext
    {
        /// <summary>
        /// Gets the navigation service.
        /// </summary>
        INavigationService NavigationService { get; }

        /// <summary>
        /// Gets the ViewModel factory.
        /// </summary>
        IViewModelFactory ViewModelFactory { get; }

        /// <summary>
        /// Gets the View locator.
        /// </summary>
        IViewLocator ViewLocator { get; }

        /// <summary>
        /// Gets the navigation journal.
        /// </summary>
        INavigationJournal NavigationJournal { get; }

        /// <summary>
        /// Gets the navigation history.
        /// </summary>
        INavigationHistory NavigationHistory { get; }
    }
}