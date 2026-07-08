using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Provides access to the application's navigation service.
    /// </summary>
    public static class NavigationManager
    {
        private static INavigationService? _current;

        /// <summary>
        /// Gets the current navigation service.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the navigation service has not been initialized.
        /// </exception>
        public static INavigationService Current =>
            _current ?? throw new InvalidOperationException(
                "Navigation service has not been initialized.");

        /// <summary>
        /// Initializes the navigation manager.
        /// This method can only be called once.
        /// </summary>
        /// <param name="navigationService">Navigation service instance.</param>
        public static void Initialize(INavigationService navigationService)
        {
            ArgumentNullException.ThrowIfNull(navigationService);

            if (_current != null)
            {
                throw new InvalidOperationException(
                    "Navigation service is already initialized.");
            }

            _current = navigationService;
        }

        /// <summary>
        /// Resets the current navigation service.
        /// Intended for testing scenarios only.
        /// </summary>
        public static void Reset()
        {
            _current = null;
        }
    }
}