using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ImageReviewModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ExposureId { get; set; }

    public string ImageName { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public string Reviewer { get; set; } = string.Empty;

    public DateTime ReviewDate { get; set; } = DateTime.Now;

    public string Result { get; set; } = "Pending";

    public string DefectType { get; set; } = string.Empty;

    public string DefectLocation { get; set; } = string.Empty;

    public double DefectLength { get; set; }

    public double DefectWidth { get; set; }

    public string AcceptanceCode { get; set; } = string.Empty;

    public string Remarks { get; set; } = string.Empty;

    public bool IsAccepted { get; set; }

    public bool IsReviewed { get; set; }

    public int ZoomLevel { get; set; } = 100;

    public double Brightness { get; set; }

    public double Contrast { get; set; }
}