using System;
using System.Collections.Generic;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationQueueService"/>.
    /// </summary>
    public sealed class NavigationQueueService : INavigationQueueService
    {
        private readonly Queue<NavigationRequest> _queue = new();

        /// <inheritdoc/>
        public bool HasPendingRequests => _queue.Count > 0;

        /// <inheritdoc/>
        public void Enqueue(NavigationRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            _queue.Enqueue(request);
        }

        /// <inheritdoc/>
        public NavigationRequest? Dequeue()
        {
            return _queue.Count > 0
                ? _queue.Dequeue()
                : null;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _queue.Clear();
        }
    }
}