using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationModeService"/>.
    /// </summary>
    public sealed class NavigationModeService : INavigationModeService
    {
        /// <inheritdoc/>
        public string Mode { get; private set; } = "Default";

        /// <inheritdoc/>
        public void SetMode(string mode)
        {
            if (string.IsNullOrWhiteSpace(mode))
            {
                throw new ArgumentException(
                    "Navigation mode cannot be empty.",
                    nameof(mode));
            }

            Mode = mode;
        }
    }
}