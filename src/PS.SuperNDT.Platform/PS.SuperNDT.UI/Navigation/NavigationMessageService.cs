using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationMessageService"/>.
    /// </summary>
    public sealed class NavigationMessageService : INavigationMessageService
    {
        /// <inheritdoc/>
        public string? Message { get; private set; }

        /// <inheritdoc/>
        public void SetMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                throw new ArgumentException(
                    "Message cannot be empty.",
                    nameof(message));
            }

            Message = message;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            Message = null;
        }
    }
}