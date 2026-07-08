using System;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Represents a navigation journal entry.
    /// </summary>
    public sealed class NavigationJournalEntry
    {
        public NavigationJournalEntry(Type viewModelType, object? parameter = null)
        {
            ViewModelType = viewModelType ?? throw new ArgumentNullException(nameof(viewModelType));
            Parameter = parameter;
            Timestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets the destination ViewModel type.
        /// </summary>
        public Type ViewModelType { get; }

        /// <summary>
        /// Gets the navigation parameter.
        /// </summary>
        public object? Parameter { get; }

        /// <summary>
        /// Gets the time when the navigation entry was created.
        /// </summary>
        public DateTime Timestamp { get; }
    }
}