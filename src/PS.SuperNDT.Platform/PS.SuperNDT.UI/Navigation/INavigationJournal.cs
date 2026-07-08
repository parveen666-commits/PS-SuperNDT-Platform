using System.Collections.Generic;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Represents a navigation journal.
    /// </summary>
    public interface INavigationJournal
    {
        /// <summary>
        /// Gets the number of journal entries.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets all journal entries.
        /// </summary>
        IReadOnlyList<NavigationJournalEntry> Entries { get; }

        /// <summary>
        /// Adds a new navigation entry.
        /// </summary>
        /// <param name="entry">The journal entry.</param>
        void Add(NavigationJournalEntry entry);

        /// <summary>
        /// Clears the journal.
        /// </summary>
        void Clear();
    }
}