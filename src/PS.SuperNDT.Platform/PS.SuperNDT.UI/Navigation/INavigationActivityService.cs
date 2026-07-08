using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for tracking navigation activity history.
    /// </summary>
    public interface INavigationActivityService
    {
        /// <summary>
        /// Gets the time of the last navigation activity.
        /// </summary>
        DateTime? LastActivity { get; }

        /// <summary>
        /// Records a navigation activity.
        /// </summary>
        void Record();

        /// <summary>
        /// Clears the activity record.
        /// </summary>
        void Clear();
    }
}