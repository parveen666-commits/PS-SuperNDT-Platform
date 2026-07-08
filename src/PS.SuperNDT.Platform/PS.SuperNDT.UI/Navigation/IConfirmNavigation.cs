using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Marks a ViewModel as supporting navigation confirmation.
    /// </summary>
    public interface IConfirmNavigation
    {
        /// <summary>
        /// Determines whether navigation away from the current ViewModel is allowed.
        /// </summary>
        /// <returns>True if navigation can proceed; otherwise false.</returns>
        bool CanNavigateAway();

        /// <summary>
        /// Determines whether navigation to this ViewModel is allowed.
        /// </summary>
        /// <param name="parameter">Optional navigation parameter.</param>
        /// <returns>True if navigation can proceed; otherwise false.</returns>
        bool CanNavigateTo(object? parameter);
    }
}