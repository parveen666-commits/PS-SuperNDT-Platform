using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationSessionService"/>.
    /// </summary>
    public sealed class NavigationSessionService : INavigationSessionService
    {
        /// <inheritdoc/>
        public Guid SessionId { get; private set; }

        /// <inheritdoc/>
        public void StartSession()
        {
            SessionId = Guid.NewGuid();
        }

        /// <inheritdoc/>
        public void EndSession()
        {
            SessionId = Guid.Empty;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            SessionId = Guid.Empty;
        }
    }
}