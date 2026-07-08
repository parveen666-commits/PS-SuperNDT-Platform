using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for tracking navigation user activity.
    /// </summary>
    public interface INavigationUserActivityService
    {
        /// <summary>
        /// Gets the last user activity time.
        /// </summary>
        DateTime? LastActivityTime { get; }

        /// <summary>
        /// Records user navigation activity.
        /// </summary>
        void RecordActivity();

        /// <summary>
        /// Clears user activity information.
        /// </summary>
        void Clear();
    }
}