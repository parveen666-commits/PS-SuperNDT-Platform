using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportStatisticsModel
{
    public int TotalReports { get; set; }

    public int CompletedReports { get; set; }

    public int PendingReports { get; set; }

    public int ApprovedReports { get; set; }

    public int RejectedReports { get; set; }

    public int ArchivedReports { get; set; }

    public int TotalImages { get; set; }

    public int TotalAnnotations { get; set; }

    public long TotalStorageBytes { get; set; }

    public DateTime GeneratedOn { get; set; }

    public string GeneratedBy { get; set; } = string.Empty;
}