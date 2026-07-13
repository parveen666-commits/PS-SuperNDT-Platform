using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportHistoryModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ReportId { get; set; }

    public string ReportNumber { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public string PerformedBy { get; set; } = string.Empty;

    public string Remarks { get; set; } = string.Empty;

    public DateTime PerformedOn { get; set; } = DateTime.Now;

    public string Version { get; set; } = "1.0";
}