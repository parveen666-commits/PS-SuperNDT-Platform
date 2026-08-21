using System;

namespace PS.SuperNDT.UI.Models;

public sealed class DefectModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ImageId { get; set; }

    public Guid JobId { get; set; }

    public int ShotNumber { get; set; }

    public string DefectType { get; set; } = "UNCLASSIFIED";

    public string Description { get; set; } = "";

    // Image coordinates
    public double X { get; set; }

    public double Y { get; set; }

    public double Width { get; set; }

    public double Height { get; set; }

    // Pipe / shot coordinates
    public double PipePosition { get; set; }

    public double PipeLength { get; set; }

    public double ShotStartPosition { get; set; }

    public double ShotEndPosition { get; set; }

    // Review information
    public string Severity { get; set; } = "UNCLASSIFIED";

    public string Status { get; set; } = "OPEN";

    public string CreatedBy { get; set; } = "";

    public DateTime CreatedOn { get; set; } = DateTime.Now;

    public string UpdatedBy { get; set; } = "";

    public DateTime? UpdatedOn { get; set; }
}