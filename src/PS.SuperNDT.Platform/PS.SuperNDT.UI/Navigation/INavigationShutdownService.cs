using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for handling navigation shutdown operations.
    /// </summary>
    public interface INavigationShutdownService
    {
        /// <summary>
        /// Gets whether navigation has been shut down.
        /// </summary>
        bool IsShutdown { get; }

        /// <summary>
        /// Shuts down navigation services.
        /// </summary>
        void Shutdown();

        /// <summary>
        /// Resets shutdown state.
        /// </summary>
        void Reset();
    }
}