using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for controlling navigation availability.
    /// </summary>
    public interface INavigationAvailabilityService
    {
        /// <summary>
        /// Gets whether navigation is available.
        /// </summary>
        bool IsAvailable { get; }

        /// <summary>
        /// Enables navigation.
        /// </summary>
        void Enable();

        /// <summary>
        /// Disables navigation.
        /// </summary>
        void Disable();
    }
}