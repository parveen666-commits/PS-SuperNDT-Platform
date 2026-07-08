using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for navigation state persistence.
    /// </summary>
    public interface INavigationPersistenceService
    {
        /// <summary>
        /// Saves the current navigation state.
        /// </summary>
        /// <param name="state">Navigation state to save.</param>
        void Save(INavigationState state);

        /// <summary>
        /// Loads the previously saved navigation state.
        /// </summary>
        /// <returns>Saved navigation state snapshot.</returns>
        NavigationSnapshot? Load();

        /// <summary>
        /// Clears the saved navigation state.
        /// </summary>
        void Clear();
    }
}