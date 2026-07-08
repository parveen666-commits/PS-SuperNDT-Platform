using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationErrorHandler"/>.
    /// </summary>
    public sealed class NavigationErrorHandler : INavigationErrorHandler
    {
        /// <inheritdoc/>
        public Exception? LastError { get; private set; }

        /// <inheritdoc/>
        public void Handle(Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);

            LastError = exception;
        }
    }
}