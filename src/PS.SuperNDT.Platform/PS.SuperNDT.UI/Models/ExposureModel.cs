using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ExposureModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid WeldId { get; set; }

    public string ExposureNumber { get; set; } = string.Empty;

    public string SourceType { get; set; } = "X-Ray";

    public string SourceSerialNumber { get; set; } = string.Empty;

    public double Kvp { get; set; }

    public double Ma { get; set; }

    public double ExposureTime { get; set; }

    public double SourceToFilmDistance { get; set; }

    public double SourceToDetectorDistance { get; set; }

    public string Technique { get; set; } = string.Empty;

    public string IQI { get; set; } = string.Empty;

    public string Filter { get; set; } = string.Empty;

    public string Detector { get; set; } = string.Empty;

    public string Operator { get; set; } = string.Empty;

    public string Result { get; set; } = "Pending";

    public string Remarks { get; set; } = string.Empty;

    public DateTime ExposureDateTime { get; set; } = DateTime.Now;

    public bool IsCompleted { get; set; }
}