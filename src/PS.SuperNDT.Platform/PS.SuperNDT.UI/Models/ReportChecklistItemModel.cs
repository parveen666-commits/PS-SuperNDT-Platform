using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportChecklistItemModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ReportId { get; set; }

    public string ChecklistName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    public string CheckedBy { get; set; } = string.Empty;

    public DateTime CheckedOn { get; set; } = DateTime.Now;

    public string Remarks { get; set; } = string.Empty;
}