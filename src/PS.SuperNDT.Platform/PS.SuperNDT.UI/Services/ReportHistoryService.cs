using System;
using System.Collections.ObjectModel;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportHistoryService
{
    private readonly ObservableCollection<ReportHistoryModel> _history;

    public ReportHistoryService()
    {
        _history = new ObservableCollection<ReportHistoryModel>();
    }

    public ReadOnlyObservableCollection<ReportHistoryModel> History =>
        new(_history);

    public void AddEntry(
        Guid reportId,
        string reportNumber,
        string action,
        string performedBy,
        string remarks)
    {
        _history.Add(
            new ReportHistoryModel
            {
                ReportId = reportId,
                ReportNumber = reportNumber,
                Action = action,
                PerformedBy = performedBy,
                Remarks = remarks,
                PerformedOn = DateTime.Now,
                Version = "1.0"
            });
    }
}