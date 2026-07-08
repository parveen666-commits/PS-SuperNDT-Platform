using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for tracking navigation session information.
    /// </summary>
    public interface INavigationSessionService
    {
        /// <summary>
        /// Gets the current session identifier.
        /// </summary>
        Guid SessionId { get; }

        /// <summary>
        /// Starts a new navigation session.
        /// </summary>
        void StartSession();

        /// <summary>
        /// Ends the current navigation session.
        /// </summary>
        void EndSession();

        /// <summary>
        /// Resets the session information.
        /// </summary>
        void Reset();
    }
}
