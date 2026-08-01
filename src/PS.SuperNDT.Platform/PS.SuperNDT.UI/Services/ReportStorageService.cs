using System;
using System.Collections.Generic;
using System.Linq;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportStorageService
{
    private readonly List<ReportDataModel> _reports = new();


    public IReadOnlyList<ReportDataModel> GetAll()
    {
        return _reports;
    }


    public ReportDataModel? GetById(
        Guid id)
    {
        return _reports.FirstOrDefault(
            x => x.Id == id);
    }


    public ReportDataModel? GetByNumber(
        string reportNumber)
    {
        return _reports.FirstOrDefault(
            x =>
                string.Equals(
                    x.ReportNumber,
                    reportNumber,
                    StringComparison.OrdinalIgnoreCase));
    }


    public void Save(
        ReportDataModel report)
    {
        ArgumentNullException.ThrowIfNull(report);


        var existing =
            _reports.FirstOrDefault(
                x => x.Id == report.Id);


        if (existing != null)
        {
            _reports.Remove(existing);
        }


        _reports.Add(report);
    }


    public void Delete(
        Guid id)
    {
        var report =
            GetById(id);


        if (report != null)
        {
            _reports.Remove(report);
        }
    }


    public void Clear()
    {
        _reports.Clear();
    }
}