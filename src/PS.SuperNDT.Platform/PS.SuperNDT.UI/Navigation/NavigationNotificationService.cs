using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationNotificationService"/>.
    /// </summary>
    public sealed class NavigationNotificationService : INavigationNotificationService
    {
        /// <inheritdoc/>
        public string? LastNotification { get; private set; }

        /// <inheritdoc/>
        public void Notify(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException(
                    "Notification message cannot be empty.",
                    nameof(message));
            }

            LastNotification = message;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            LastNotification = null;
        }
    }
}