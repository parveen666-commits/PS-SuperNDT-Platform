using System;

namespace PS.SuperNDT.UI.Models;

public sealed class DefectModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ImageId { get; set; }

    public Guid JobId { get; set; }

    public int ShotNumber { get; set; }

    // ============================================================
    // DEFECT INFORMATION
    // ============================================================

    public string DefectType { get; set; } = "UNCLASSIFIED";

    public string Description { get; set; } = "";

    public string Severity { get; set; } = "UNCLASSIFIED";

    public string Status { get; set; } = "OPEN";

    // ============================================================
    // IMAGE COORDINATES
    // ============================================================

    public double X { get; set; }

    public double Y { get; set; }

    public double Width { get; set; }

    public double Height { get; set; }

    // ============================================================
    // DEFECT MEASUREMENT
    // ============================================================

    public double LengthMm { get; set; }

    public double WidthMm { get; set; }

    // ============================================================
    // PIPE / SHOT COORDINATES
    // ============================================================

    public double PipePosition { get; set; }

    public double PipeLength { get; set; }

    public double ShotStartPosition { get; set; }

    public double ShotEndPosition { get; set; }

    // ============================================================
    // THICKNESS CHECK
    // ============================================================

    public bool ThicknessChecked { get; set; }

    public double NominalThicknessMm { get; set; }

    public double ActualThicknessMm { get; set; }

    public double MinimumThicknessMm { get; set; }

    public string ThicknessStatus { get; set; } = "NOT CHECKED";

    public string ThicknessRemark { get; set; } = "";

    // ============================================================
    // AUDIT
    // ============================================================

    public string CreatedBy { get; set; } = "";

    public DateTime CreatedOn { get; set; } = DateTime.Now;

    public string UpdatedBy { get; set; } = "";

    public DateTime? UpdatedOn { get; set; }
}