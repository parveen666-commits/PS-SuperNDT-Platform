using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationTransitionTracker"/>.
    /// </summary>
    public sealed class NavigationTransitionTracker : INavigationTransitionTracker
    {
        /// <inheritdoc/>
        public Type? LastSource { get; private set; }

        /// <inheritdoc/>
        public Type? LastTarget { get; private set; }

        /// <inheritdoc/>
        public void Track(Type? source, Type target)
        {
            ArgumentNullException.ThrowIfNull(target);

            LastSource = source;
            LastTarget = target;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            LastSource = null;
            LastTarget = null;
        }
    }
}