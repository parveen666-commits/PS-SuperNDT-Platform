using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for managing navigation startup state.
    /// </summary>
    public interface INavigationStartupService
    {
        /// <summary>
        /// Gets whether navigation has been initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Initializes navigation services.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Resets navigation startup state.
        /// </summary>
        void Reset();
    }
}