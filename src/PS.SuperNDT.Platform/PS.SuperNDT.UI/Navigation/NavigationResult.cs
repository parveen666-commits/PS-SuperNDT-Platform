using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Represents a navigation result.
    /// </summary>
    public sealed class NavigationResult
    {
        public NavigationResult(bool succeeded)
            : this(succeeded, null)
        {
        }

        public NavigationResult(bool succeeded, string? errorMessage)
        {
            Succeeded = succeeded;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Gets a value indicating whether the navigation succeeded.
        /// </summary>
        public bool Succeeded { get; }

        /// <summary>
        /// Gets a value indicating whether the navigation failed.
        /// </summary>
        public bool Failed => !Succeeded;

        /// <summary>
        /// Gets the error message when navigation fails.
        /// </summary>
        public string? ErrorMessage { get; }

        /// <summary>
        /// Creates a successful navigation result.
        /// </summary>
        public static NavigationResult Success()
        {
            return new NavigationResult(true);
        }

        /// <summary>
        /// Creates a failed navigation result.
        /// </summary>
        public static NavigationResult Failure(string errorMessage)
        {
            ArgumentNullException.ThrowIfNull(errorMessage);

            return new NavigationResult(false, errorMessage);
        }
    }
}