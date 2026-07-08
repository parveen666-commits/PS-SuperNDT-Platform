using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationResultService"/>.
    /// </summary>
    public sealed class NavigationResultService : INavigationResultService
    {
        /// <inheritdoc/>
        public NavigationResult? LastResult { get; private set; }

        /// <inheritdoc/>
        public void SetResult(NavigationResult result)
        {
            ArgumentNullException.ThrowIfNull(result);

            LastResult = result;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            LastResult = null;
        }
    }
}