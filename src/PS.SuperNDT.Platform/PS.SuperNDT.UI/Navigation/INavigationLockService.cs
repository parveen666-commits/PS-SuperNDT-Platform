using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service that controls navigation locking state.
    /// </summary>
    public interface INavigationLockService
    {
        /// <summary>
        /// Gets whether navigation is currently locked.
        /// </summary>
        bool IsLocked { get; }

        /// <summary>
        /// Locks navigation.
        /// </summary>
        void Lock();

        /// <summary>
        /// Unlocks navigation.
        /// </summary>
        void Unlock();
    }
}