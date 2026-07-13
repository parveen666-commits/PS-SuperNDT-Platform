using System;

namespace PS.SuperNDT.UI.Models;

public sealed class WeldModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid JobId { get; set; }

    public string WeldNumber { get; set; } = string.Empty;

    public string SpoolNumber { get; set; } = string.Empty;

    public string LineNumber { get; set; } = string.Empty;

    public string JointType { get; set; } = string.Empty;

    public string Material { get; set; } = string.Empty;

    public double Diameter { get; set; }

    public double Thickness { get; set; }

    public string Schedule { get; set; } = string.Empty;

    public string Technique { get; set; } = string.Empty;

    public string InspectionStatus { get; set; } = "Pending";

    public int TotalShots { get; set; }

    public int AcceptedShots { get; set; }

    public int RejectedShots { get; set; }

    public string Remarks { get; set; } = string.Empty;

    public DateTime CreatedOn { get; set; } = DateTime.Now;
}