using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Provides navigation state information.
    /// </summary>
    public interface INavigationState
    {
        /// <summary>
        /// Gets the current ViewModel instance.
        /// </summary>
        object? CurrentViewModel { get; }

        /// <summary>
        /// Gets the previous ViewModel instance.
        /// </summary>
        object? PreviousViewModel { get; }

        /// <summary>
        /// Gets the time of the last successful navigation.
        /// </summary>
        DateTime? LastNavigationTime { get; }
    }
}