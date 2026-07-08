using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for managing navigation messages.
    /// </summary>
    public interface INavigationMessageService
    {
        /// <summary>
        /// Gets the current navigation message.
        /// </summary>
        string? Message { get; }

        /// <summary>
        /// Sets a navigation message.
        /// </summary>
        /// <param name="message">Navigation message.</param>
        void SetMessage(string message);

        /// <summary>
        /// Clears the navigation message.
        /// </summary>
        void Clear();
    }
}