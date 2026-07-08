using System.Threading.Tasks;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Supports asynchronous navigation lifecycle notifications.
    /// </summary>
    public interface IAsyncNavigationAware
    {
        /// <summary>
        /// Called after navigation to the ViewModel.
        /// </summary>
        /// <param name="parameter">Optional navigation parameter.</param>
        Task OnNavigatedToAsync(object? parameter);

        /// <summary>
        /// Called before navigating away from the ViewModel.
        /// </summary>
        Task OnNavigatedFromAsync();
    }
}