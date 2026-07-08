using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationSnapshotService"/>.
    /// </summary>
    public sealed class NavigationSnapshotService : INavigationSnapshotService
    {
        private readonly INavigationState _navigationState;

        public NavigationSnapshotService(INavigationState navigationState)
        {
            _navigationState = navigationState ?? throw new ArgumentNullException(nameof(navigationState));
        }

        /// <inheritdoc/>
        public NavigationSnapshot CreateSnapshot()
        {
            return new NavigationSnapshot(
                _navigationState.CurrentViewModel,
                _navigationState.PreviousViewModel,
                DateTime.UtcNow);
        }

        /// <inheritdoc/>
        public void RestoreSnapshot(NavigationSnapshot snapshot)
        {
            ArgumentNullException.ThrowIfNull(snapshot);

            if (_navigationState is NavigationState state)
            {
                state.Update(snapshot.CurrentViewModel);
            }
        }
    }
}