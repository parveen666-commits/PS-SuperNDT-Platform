using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Event arguments for ViewModel navigation.
    /// </summary>
    public sealed class ViewModelNavigatedEventArgs : EventArgs
    {
        public ViewModelNavigatedEventArgs(
            Type viewModelType,
            object? viewModel,
            object? parameter = null)
        {
            ViewModelType = viewModelType ?? throw new ArgumentNullException(nameof(viewModelType));
            ViewModel = viewModel;
            Parameter = parameter;
        }

        /// <summary>
        /// Gets the ViewModel type.
        /// </summary>
        public Type ViewModelType { get; }

        /// <summary>
        /// Gets the ViewModel instance.
        /// </summary>
        public object? ViewModel { get; }

        /// <summary>
        /// Gets the navigation parameter.
        /// </summary>
        public object? Parameter { get; }
    }
}