using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for validating navigation lifecycle state.
    /// </summary>
    public interface INavigationLifecycleService
    {
        /// <summary>
        /// Gets whether navigation lifecycle is active.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Starts navigation lifecycle.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops navigation lifecycle.
        /// </summary>
        void Stop();
    }
}