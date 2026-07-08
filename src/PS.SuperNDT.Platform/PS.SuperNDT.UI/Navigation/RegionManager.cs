using System;
using System.Collections.Generic;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="IRegionManager"/>.
    /// </summary>
    public sealed class RegionManager : IRegionManager
    {
        private readonly Dictionary<string, INavigationRegion> _regions =
            new(StringComparer.OrdinalIgnoreCase);

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, INavigationRegion> Regions => _regions;

        /// <inheritdoc/>
        public void RegisterRegion(INavigationRegion region)
        {
            ArgumentNullException.ThrowIfNull(region);

            _regions[region.Name] = region;
        }

        /// <inheritdoc/>
        public INavigationRegion? GetRegion(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            return _regions.TryGetValue(name, out var region)
                ? region
                : null;
        }
    }
}