using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for registering ViewModel factories.
    /// </summary>
    public interface IViewModelRegistry
    {
        /// <summary>
        /// Registers a factory for the specified ViewModel type.
        /// </summary>
        /// <typeparam name="TViewModel">The ViewModel type.</typeparam>
        /// <param name="factory">Factory used to create the ViewModel.</param>
        void Register<TViewModel>(Func<TViewModel> factory)
            where TViewModel : class;

        /// <summary>
        /// Determines whether the specified ViewModel type is registered.
        /// </summary>
        /// <typeparam name="TViewModel">The ViewModel type.</typeparam>
        /// <returns>True if registered; otherwise false.</returns>
        bool IsRegistered<TViewModel>()
            where TViewModel : class;
    }
}
