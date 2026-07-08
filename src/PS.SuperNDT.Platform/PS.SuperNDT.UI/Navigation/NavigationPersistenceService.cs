using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationPersistenceService"/>.
    /// </summary>
    public sealed class NavigationPersistenceService : INavigationPersistenceService
    {
        private NavigationSnapshot? _snapshot;

        /// <inheritdoc/>
        public void Save(INavigationState state)
        {
            ArgumentNullException.ThrowIfNull(state);

            _snapshot = new NavigationSnapshot(
                state.CurrentViewModel,
                state.PreviousViewModel,
                DateTime.UtcNow);
        }

        /// <inheritdoc/>
        public NavigationSnapshot? Load()
        {
            return _snapshot;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _snapshot = null;
        }
    }
}