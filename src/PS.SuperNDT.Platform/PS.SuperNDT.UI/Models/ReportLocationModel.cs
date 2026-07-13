using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportLocationModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ReportId { get; set; }

    public string ComponentLocation { get; set; } = string.Empty;

    public string WeldLocation { get; set; } = string.Empty;

    public string JointNumber { get; set; } = string.Empty;

    public string DrawingReference { get; set; } = string.Empty;

    public string Area { get; set; } = string.Empty;

    public string Remarks { get; set; } = string.Empty;
}