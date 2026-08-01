using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportAuditModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ReportId { get; set; }

    public string ReportNumber { get; set; } = string.Empty;

    public string Action { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string OldValue { get; set; } = string.Empty;

    public string NewValue { get; set; } = string.Empty;

    public string PerformedBy { get; set; } = string.Empty;

    public DateTime PerformedOn { get; set; } = DateTime.Now;

    public string Remarks { get; set; } = string.Empty;
}