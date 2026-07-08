using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationScope"/>.
    /// </summary>
    public sealed class NavigationScope : INavigationScope
    {
        public NavigationScope(
            string name,
            INavigationContext context)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "Scope name cannot be empty.",
                    nameof(name));
            }

            Name = name;
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public INavigationContext Context { get; }
    }
}