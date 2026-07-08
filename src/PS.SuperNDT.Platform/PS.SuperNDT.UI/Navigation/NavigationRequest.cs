using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Represents a navigation request within the application.
    /// </summary>
    public sealed class NavigationRequest
    {
        public NavigationRequest(Type viewModelType)
            : this(viewModelType, null)
        {
        }

        public NavigationRequest(Type viewModelType, object? parameter)
        {
            ViewModelType = viewModelType ?? throw new ArgumentNullException(nameof(viewModelType));
            Parameter = parameter;
        }

        /// <summary>
        /// Gets the destination ViewModel type.
        /// </summary>
        public Type ViewModelType { get; }

        /// <summary>
        /// Gets the optional parameter supplied during navigation.
        /// </summary>
        public object? Parameter { get; }
    }
}