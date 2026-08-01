using System;
using System.Collections.Generic;
using System.Linq;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportStatisticsService
{
    public ReportStatisticsModel Generate(IEnumerable<ReportModel> reports)
    {
        ArgumentNullException.ThrowIfNull(reports);

        var reportList = reports.ToList();

        return new ReportStatisticsModel
        {
            TotalReports = reportList.Count,
            CompletedReports = reportList.Count(r =>
                string.Equals(r.Result, "ACCEPTED", StringComparison.OrdinalIgnoreCase)),
            PendingReports = reportList.Count(r =>
                string.Equals(r.Result, "PENDING", StringComparison.OrdinalIgnoreCase)),
            ApprovedReports = reportList.Count(r =>
                string.Equals(r.Result, "APPROVED", StringComparison.OrdinalIgnoreCase)),
            RejectedReports = reportList.Count(r =>
                string.Equals(r.Result, "REJECTED", StringComparison.OrdinalIgnoreCase)),
            ArchivedReports = 0,
            TotalImages = 0,
            TotalAnnotations = 0,
            TotalStorageBytes = 0,
            GeneratedOn = DateTime.Now,
            GeneratedBy = Environment.UserName
        };
    }
}