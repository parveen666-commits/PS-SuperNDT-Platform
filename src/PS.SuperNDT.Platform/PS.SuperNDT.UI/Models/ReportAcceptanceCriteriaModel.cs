using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportAcceptanceCriteriaModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ReportId { get; set; }

    public string Standard { get; set; } = string.Empty;

    public string AcceptanceLevel { get; set; } = string.Empty;

    public string CriteriaDescription { get; set; } = string.Empty;

    public bool IsAccepted { get; set; }

    public string EvaluatedBy { get; set; } = string.Empty;

    public DateTime EvaluatedOn { get; set; } = DateTime.Now;

    public string Remarks { get; set; } = string.Empty;
}