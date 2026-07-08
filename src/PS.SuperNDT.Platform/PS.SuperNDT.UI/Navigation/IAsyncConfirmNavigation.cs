using System.Threading.Tasks;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Extends navigation confirmation with asynchronous support.
    /// </summary>
    public interface IAsyncConfirmNavigation
    {
        /// <summary>
        /// Determines whether navigation to the ViewModel is allowed.
        /// </summary>
        /// <param name="parameter">Optional navigation parameter.</param>
        /// <returns>True if navigation is allowed; otherwise false.</returns>
        Task<bool> CanNavigateToAsync(object? parameter);

        /// <summary>
        /// Determines whether navigation away from the ViewModel is allowed.
        /// </summary>
        /// <returns>True if navigation is allowed; otherwise false.</returns>
        Task<bool> CanNavigateAwayAsync();
    }
}