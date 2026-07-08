using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationContext"/>.
    /// </summary>
    public sealed class NavigationContext : INavigationContext
    {
        public NavigationContext(
            INavigationService navigationService,
            IViewModelFactory viewModelFactory,
            IViewLocator viewLocator,
            INavigationJournal navigationJournal,
            INavigationHistory navigationHistory)
        {
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            ViewModelFactory = viewModelFactory ?? throw new ArgumentNullException(nameof(viewModelFactory));
            ViewLocator = viewLocator ?? throw new ArgumentNullException(nameof(viewLocator));
            NavigationJournal = navigationJournal ?? throw new ArgumentNullException(nameof(navigationJournal));
            NavigationHistory = navigationHistory ?? throw new ArgumentNullException(nameof(navigationHistory));
        }

        /// <inheritdoc/>
        public INavigationService NavigationService { get; }

        /// <inheritdoc/>
        public IViewModelFactory ViewModelFactory { get; }

        /// <inheritdoc/>
        public IViewLocator ViewLocator { get; }

        /// <inheritdoc/>
        public INavigationJournal NavigationJournal { get; }

        /// <inheritdoc/>
        public INavigationHistory NavigationHistory { get; }
    }
}