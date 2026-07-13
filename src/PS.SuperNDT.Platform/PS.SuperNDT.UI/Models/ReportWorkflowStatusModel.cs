using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportWorkflowStatusModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ReportId { get; set; }

    public string Status { get; set; } = "Draft";

    public string CurrentStage { get; set; } = "Report Creation";

    public string UpdatedBy { get; set; } = string.Empty;

    public DateTime UpdatedOn { get; set; } = DateTime.Now;

    public bool IsCompleted { get; set; }
}