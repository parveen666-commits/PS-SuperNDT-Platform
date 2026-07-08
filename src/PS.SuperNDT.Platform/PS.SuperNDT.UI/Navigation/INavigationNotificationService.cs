using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for tracking navigation notifications.
    /// </summary>
    public interface INavigationNotificationService
    {
        /// <summary>
        /// Gets the latest navigation notification.
        /// </summary>
        string? LastNotification { get; }

        /// <summary>
        /// Publishes a navigation notification.
        /// </summary>
        /// <param name="message">Notification message.</param>
        void Notify(string message);

        /// <summary>
        /// Clears the current notification.
        /// </summary>
        void Clear();
    }
}