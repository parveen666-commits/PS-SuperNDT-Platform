using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ShotPlanItemModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ShotPlanId { get; set; }

    public Guid JobId { get; set; }

    public string PipeId { get; set; } = string.Empty;

    public string WeldNumber { get; set; } = string.Empty;

    public int ShotNumber { get; set; }

    public double StartPositionMm { get; set; }

    public double EndPositionMm { get; set; }

    public double NominalShotLengthMm { get; set; }

    public double ActualCoverageMm { get; set; }

    public double OverlapMm { get; set; }

    public double RulerStartMm { get; set; }

    public double RulerEndMm { get; set; }

    public string Status { get; set; } = "Pending";

    public string AcquisitionMode { get; set; } = "Manual";

    public bool IsCaptured { get; set; }

    public bool IsReviewed { get; set; }

    public bool IsAccepted { get; set; }

    public Guid? ImageId { get; set; }

    public string ImageFileName { get; set; } = string.Empty;

    public DateTime? CapturedOn { get; set; }

    public string Remarks { get; set; } = string.Empty;

    public string PositionText =>
        $"{StartPositionMm:0.###} → {EndPositionMm:0.###} mm";

    public string RulerText =>
        $"{RulerStartMm:0.###} → {RulerEndMm:0.###} mm";
}