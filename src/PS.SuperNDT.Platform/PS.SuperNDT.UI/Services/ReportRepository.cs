using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportRepository
{
    private readonly string _reportFile =
        Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "reports.json");

    public List<ReportModel> GetAll()
    {
        try
        {
            if (!File.Exists(_reportFile))
            {
                return new List<ReportModel>();
            }

            var json =
                File.ReadAllText(_reportFile);

            var reports =
                JsonSerializer.Deserialize<List<ReportModel>>(json);

            return reports ??
                   new List<ReportModel>();
        }
        catch
        {
            return new List<ReportModel>();
        }
    }

    public ReportModel? GetByReportNumber(
        string reportNumber)
    {
        return GetAll()
            .FirstOrDefault(x =>
                x.ReportNumber.Equals(
                    reportNumber,
                    StringComparison.OrdinalIgnoreCase));
    }

    public void AddOrUpdate(
        ReportModel report)
    {
        var reports = GetAll();

        var existing =
            reports.FirstOrDefault(x =>
                x.ReportNumber.Equals(
                    report.ReportNumber,
                    StringComparison.OrdinalIgnoreCase));

        if (existing == null)
        {
            reports.Add(report);
        }
        else
        {
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

        Save(reports);
    }

    public void Delete(
        string reportNumber)
    {
        var reports = GetAll();

        reports.RemoveAll(x =>
            x.ReportNumber.Equals(
                reportNumber,
                StringComparison.OrdinalIgnoreCase));

        Save(reports);
    }

    private void Save(
        List<ReportModel> reports)
    {
        var json =
            JsonSerializer.Serialize(
                reports,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

        File.WriteAllText(
            _reportFile,
            json);
    }
}