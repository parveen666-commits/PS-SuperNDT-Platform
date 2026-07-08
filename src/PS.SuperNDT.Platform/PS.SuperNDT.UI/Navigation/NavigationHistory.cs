using System.Collections.Generic;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default in-memory navigation history implementation.
    /// </summary>
    public sealed class NavigationHistory : INavigationHistory
    {
        private readonly Stack<object> _history = new();

        /// <inheritdoc/>
        public int Count => _history.Count;

        /// <inheritdoc/>
        public bool CanGoBack => _history.Count > 0;

        /// <inheritdoc/>
        public void Push(object viewModel)
        {
            if (viewModel != null)
            {
                _history.Push(viewModel);
            }
        }

        /// <inheritdoc/>
        public object? Pop()
        {
            return _history.Count > 0
                ? _history.Pop()
                : null;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _history.Clear();
        }
    }
}