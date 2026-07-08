using System;
using System.Collections.Generic;

namespace PS.SuperNDT.UI.Navigation
{
    /// <summary>
    /// Default in-memory implementation of <see cref="INavigationJournal"/>.
    /// </summary>
    public sealed class NavigationJournal : INavigationJournal
    {
        private readonly List<NavigationJournalEntry> _entries = new();

        /// <inheritdoc/>
        public int Count => _entries.Count;

        /// <inheritdoc/>
        public IReadOnlyList<NavigationJournalEntry> Entries => _entries;

        /// <inheritdoc/>
        public void Add(NavigationJournalEntry entry)
        {
            ArgumentNullException.ThrowIfNull(entry);
            _entries.Add(entry);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _entries.Clear();
        }
    }
}