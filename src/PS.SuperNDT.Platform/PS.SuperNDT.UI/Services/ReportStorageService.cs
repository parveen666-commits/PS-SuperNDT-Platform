using System;
using System.Collections.Generic;
using System.Linq;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportStorageService
{
    private static readonly List<ReportDataModel> Reports = new();

    public IReadOnlyList<ReportDataModel> GetAll()
    {
        return Reports;
    }

    public ReportDataModel? GetById(Guid id)
    {
        return Reports.FirstOrDefault(x => x.Id == id);
    }

    public ReportDataModel? GetByReportNumber(string reportNumber)
    {
        return Reports.FirstOrDefault(x =>
            string.Equals(
                x.ReportNumber,
                reportNumber,
                StringComparison.OrdinalIgnoreCase));
    }

    public void Save(ReportDataModel report)
    {
        ArgumentNullException.ThrowIfNull(report);

        var existing = GetById(report.Id);

        if (existing != null)
        {
            Reports.Remove(existing);
        }

        Reports.Add(report);
    }

    public void Delete(Guid id)
    {
        var report = GetById(id);

        if (report != null)
        {
            Reports.Remove(report);
        }
    }

    public bool Exists(string reportNumber)
    {
        return Reports.Any(x =>
            string.Equals(
                x.ReportNumber,
                reportNumber,
                StringComparison.OrdinalIgnoreCase));
    }

    public void Clear()
    {
        Reports.Clear();
    }
}