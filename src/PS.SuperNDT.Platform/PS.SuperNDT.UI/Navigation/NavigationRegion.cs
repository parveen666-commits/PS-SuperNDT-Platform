using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default implementation of <see cref="INavigationRegion"/>.
    /// </summary>
    public sealed class NavigationRegion : INavigationRegion
    {
        public NavigationRegion(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException(
                    "Region name cannot be empty.",
                    nameof(name));
            }

            Name = name;
        }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public object? CurrentView { get; private set; }

        /// <inheritdoc/>
        public object? CurrentViewModel { get; private set; }

        /// <inheritdoc/>
        public void SetContent(object view, object viewModel)
        {
            ArgumentNullException.ThrowIfNull(view);
            ArgumentNullException.ThrowIfNull(viewModel);

            CurrentView = view;
            CurrentViewModel = viewModel;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            CurrentView = null;
            CurrentViewModel = null;
        }
    }
}