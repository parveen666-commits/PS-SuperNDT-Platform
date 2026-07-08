using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a service for controlling navigation requests queue.
    /// </summary>
    public interface INavigationQueueService
    {
        /// <summary>
        /// Gets whether queued navigation requests are available.
        /// </summary>
        bool HasPendingRequests { get; }

        /// <summary>
        /// Adds a navigation request to the queue.
        /// </summary>
        /// <param name="request">Navigation request.</param>
        void Enqueue(NavigationRequest request);

        /// <summary>
        /// Gets the next navigation request.
        /// </summary>
        /// <returns>The next navigation request.</returns>
        NavigationRequest? Dequeue();

        /// <summary>
        /// Clears all pending navigation requests.
        /// </summary>
        void Clear();
    }
}