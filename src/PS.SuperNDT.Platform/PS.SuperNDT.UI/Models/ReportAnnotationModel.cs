using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportAnnotationModel
{
    public Guid Id { get; set; }

    public Guid ReportId { get; set; }

    public Guid? ImageId { get; set; }

    public string AnnotationType { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public double PositionX { get; set; }

    public double PositionY { get; set; }

    public double Width { get; set; }

    public double Height { get; set; }

    public string Color { get; set; } = string.Empty;

    public string CreatedBy { get; set; } = string.Empty;

    public DateTime CreatedOn { get; set; }

    public bool IsCritical { get; set; }
}