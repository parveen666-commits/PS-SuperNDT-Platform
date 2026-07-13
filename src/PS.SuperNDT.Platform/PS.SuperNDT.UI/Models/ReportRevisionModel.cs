using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportRevisionModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ReportId { get; set; }

    public string ReportNumber { get; set; } = string.Empty;

    public string RevisionNumber { get; set; } = "Rev-00";

    public string ChangeDescription { get; set; } = string.Empty;

    public string RevisedBy { get; set; } = string.Empty;

    public DateTime RevisedOn { get; set; } = DateTime.Now;

    public bool IsCurrentRevision { get; set; } = true;
}