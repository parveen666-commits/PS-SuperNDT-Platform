using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportRevisionModel
{
    public Guid Id { get; set; }

    public Guid ReportId { get; set; }

    public string ReportNumber { get; set; } = string.Empty;

    public string RevisionNumber { get; set; } = string.Empty;

    public string ChangeDescription { get; set; } = string.Empty;

    public string RevisionDescription { get; set; } = string.Empty;

    public string PreviousVersion { get; set; } = string.Empty;

    public string CurrentVersion { get; set; } = string.Empty;

    public string RevisedBy { get; set; } = string.Empty;

    public DateTime RevisedOn { get; set; }

    public bool IsCurrentRevision { get; set; }

    public bool IsCurrent { get; set; }

    public string Remarks { get; set; } = string.Empty;
}