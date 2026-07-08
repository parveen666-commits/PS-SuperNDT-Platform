using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for controlling navigation mode.
    /// </summary>
    public interface INavigationModeService
    {
        /// <summary>
        /// Gets the current navigation mode.
        /// </summary>
        string Mode { get; }

        /// <summary>
        /// Changes the navigation mode.
        /// </summary>
        /// <param name="mode">New navigation mode.</param>
        void SetMode(string mode);
    }
}