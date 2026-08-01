using System;
using System.Collections.Generic;
using System.Linq;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportService
{
    private readonly List<ReportModel> _reports = new();

    public IReadOnlyList<ReportModel> GetAll()
    {
        return _reports;
    }

    public ReportModel? GetByReportNumber(string reportNumber)
    {
        return _reports.FirstOrDefault(x =>
            string.Equals(x.ReportNumber, reportNumber, StringComparison.OrdinalIgnoreCase));
    }

    public void Add(ReportModel report)
    {
        ArgumentNullException.ThrowIfNull(report);

        if (string.IsNullOrWhiteSpace(report.ReportNumber))
            throw new ArgumentException("Report Number is required.", nameof(report));

        if (GetByReportNumber(report.ReportNumber) != null)
            return;

        _reports.Add(report);
    }

    public void Update(ReportModel report)
    {
        ArgumentNullException.ThrowIfNull(report);

        var existing = GetByReportNumber(report.ReportNumber);

        if (existing == null)
            return;

        existing.JobNumber = report.JobNumber;
        existing.Customer = report.Customer;
        existing.Project = report.Project;
        existing.Component = report.Component;
        existing.WeldNumber = report.WeldNumber;
        existing.Inspector = report.Inspector;
        existing.ReportDate = report.ReportDate;
        existing.Result = report.Result;
        existing.Remarks = report.Remarks;
        existing.ReportFilePath = report.ReportFilePath;
    }

    public bool Delete(string reportNumber)
    {
        var report = GetByReportNumber(reportNumber);

        if (report == null)
            return false;

        return _reports.Remove(report);
    }

    public IEnumerable<ReportModel> Search(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return _reports;

        keyword = keyword.Trim();

        return _reports.Where(x =>
            x.ReportNumber.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            x.JobNumber.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            x.Customer.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            x.Project.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            x.Component.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            x.WeldNumber.Contains(keyword, StringComparison.OrdinalIgnoreCase));
    }

    public void Clear()
    {
        _reports.Clear();
    }
}