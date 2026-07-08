using System;
using System.Collections.Generic;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Defines a manager for application navigation regions.
    /// </summary>
    public interface IRegionManager
    {
        /// <summary>
        /// Gets all registered regions.
        /// </summary>
        IReadOnlyDictionary<string, INavigationRegion> Regions { get; }

        /// <summary>
        /// Registers a navigation region.
        /// </summary>
        /// <param name="region">The region to register.</param>
        void RegisterRegion(INavigationRegion region);

        /// <summary>
        /// Gets a registered region by name.
        /// </summary>
        /// <param name="name">Region name.</param>
        /// <returns>The region instance.</returns>
        INavigationRegion? GetRegion(string name);
    }
}