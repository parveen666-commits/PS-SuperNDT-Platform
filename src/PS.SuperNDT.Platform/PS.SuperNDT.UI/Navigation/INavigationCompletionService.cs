using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for handling navigation completion callbacks.
    /// </summary>
    public interface INavigationCompletionService
    {
        /// <summary>
        /// Gets the last completion time.
        /// </summary>
        DateTime? LastCompleted { get; }

        /// <summary>
        /// Marks navigation as completed.
        /// </summary>
        void Complete();

        /// <summary>
        /// Clears completion information.
        /// </summary>
        void Reset();
    }
}