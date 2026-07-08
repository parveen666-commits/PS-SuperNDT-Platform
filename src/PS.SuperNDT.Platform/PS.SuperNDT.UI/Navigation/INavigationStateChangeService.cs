using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for managing navigation state changes.
    /// </summary>
    public interface INavigationStateChangeService
    {
        /// <summary>
        /// Gets the last changed navigation state time.
        /// </summary>
        DateTime? LastChanged { get; }

        /// <summary>
        /// Records a navigation state change.
        /// </summary>
        void NotifyChanged();

        /// <summary>
        /// Clears the change information.
        /// </summary>
        void Reset();
    }
}