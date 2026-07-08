using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for handling navigation errors.
    /// </summary>
    public interface INavigationErrorHandler
    {
        /// <summary>
        /// Handles a navigation exception.
        /// </summary>
        /// <param name="exception">The exception that occurred.</param>
        void Handle(Exception exception);

        /// <summary>
        /// Gets the last navigation error.
        /// </summary>
        Exception? LastError { get; }
    }
}