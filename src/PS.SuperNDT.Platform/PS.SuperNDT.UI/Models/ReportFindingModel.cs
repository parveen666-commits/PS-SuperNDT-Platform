using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportFindingModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ReportId { get; set; }

    public int FindingNumber { get; set; }

    public string Location { get; set; } = string.Empty;

    public string FindingType { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Severity { get; set; } = string.Empty;

    public string Evaluation { get; set; } = string.Empty;

    public bool IsAccepted { get; set; }

    public string InspectorRemark { get; set; } = string.Empty;

    public DateTime CreatedOn { get; set; } = DateTime.Now;
}