using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for managing navigation commands state.
    /// </summary>
    public interface INavigationCommandState
    {
        /// <summary>
        /// Gets whether navigation command execution is enabled.
        /// </summary>
        bool CanExecute { get; }

        /// <summary>
        /// Updates command execution state.
        /// </summary>
        /// <param name="canExecute">Command execution availability.</param>
        void Update(bool canExecute);

        /// <summary>
        /// Resets command state.
        /// </summary>
        void Reset();
    }
}