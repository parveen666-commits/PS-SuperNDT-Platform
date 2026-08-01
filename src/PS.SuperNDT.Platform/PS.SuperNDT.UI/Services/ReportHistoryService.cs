using System;
using System.Collections.Generic;
using System.Linq;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportHistoryService
{
    private readonly List<ReportHistoryModel> _history = new();

    public IReadOnlyList<ReportHistoryModel> GetAll()
    {
        return _history;
    }

    public IEnumerable<ReportHistoryModel> GetByReport(string reportNumber)
    {
        return _history.Where(x =>
            string.Equals(x.ReportNumber, reportNumber, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(x => x.PerformedOn);
    }

    public void Add(ReportHistoryModel item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _history.Add(item);
    }

    public void Clear()
    {
        _history.Clear();
    }
}