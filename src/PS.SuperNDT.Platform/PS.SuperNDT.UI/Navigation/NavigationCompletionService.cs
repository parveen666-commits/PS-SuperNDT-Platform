using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationCompletionService"/>.
    /// </summary>
    public sealed class NavigationCompletionService : INavigationCompletionService
    {
        /// <inheritdoc/>
        public DateTime? LastCompleted { get; private set; }

        /// <inheritdoc/>
        public void Complete()
        {
            LastCompleted = DateTime.UtcNow;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            LastCompleted = null;
        }
    }
}