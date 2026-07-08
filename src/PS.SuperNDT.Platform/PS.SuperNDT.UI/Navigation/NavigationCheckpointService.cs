using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationCheckpointService"/>.
    /// </summary>
    public sealed class NavigationCheckpointService : INavigationCheckpointService
    {
        private readonly INavigationSnapshotService _snapshotService;
        private NavigationSnapshot? _checkpoint;

        public NavigationCheckpointService(
            INavigationSnapshotService snapshotService)
        {
            _snapshotService = snapshotService ??
                throw new ArgumentNullException(nameof(snapshotService));
        }

        /// <inheritdoc/>
        public void CreateCheckpoint()
        {
            _checkpoint = _snapshotService.CreateSnapshot();
        }

        /// <inheritdoc/>
        public void RestoreCheckpoint()
        {
            if (_checkpoint is not null)
            {
                _snapshotService.RestoreSnapshot(_checkpoint);
            }
        }

        /// <inheritdoc/>
        public void ClearCheckpoint()
        {
            _checkpoint = null;
        }
    }
}