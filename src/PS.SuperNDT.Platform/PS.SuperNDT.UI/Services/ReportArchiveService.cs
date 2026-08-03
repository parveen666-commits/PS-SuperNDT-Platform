using System;
using System.Collections.Generic;
using System.Linq;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportArchiveService
{
    private readonly List<ReportDataModel> _archivedReports = new();

    public IReadOnlyList<ReportDataModel> GetAll()
    {
        return _archivedReports;
    }

    public void Archive(ReportDataModel report)
    {
        ArgumentNullException.ThrowIfNull(report);

        if (_archivedReports.Any(x => x.Id == report.Id))
        {
            return;
        }

        _archivedReports.Add(report);

        new ReportAuditService().Record(
            report.Id,
            report.ReportNumber,
            "Archive",
            "Report archived",
            "System");
    }

    public bool IsArchived(Guid reportId)
    {
        return _archivedReports.Any(x => x.Id == reportId);
    }

    public void Remove(Guid reportId)
    {
        var report = _archivedReports.FirstOrDefault(x => x.Id == reportId);

        if (report != null)
        {
            _archivedReports.Remove(report);
        }
    }

    public void Clear()
    {
        _archivedReports.Clear();
    }
}