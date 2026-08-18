using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ImageRecordModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid JobId { get; set; }

    public string JobNumber { get; set; } = "";

    public string PipeId { get; set; } = "";

    public string Operator { get; set; } = "";

    public string Remarks { get; set; } = "";

    public int FrameNumber { get; set; }

    public string FileName { get; set; } = "";

    public string FilePath { get; set; } = "";

    public string DetectorName { get; set; } = "";

    public double KV { get; set; }

    public double MA { get; set; }

    public double ExposureTime { get; set; }

    public int ImageWidth { get; set; }

    public int ImageHeight { get; set; }

    public int BitDepth { get; set; }

    public DateTime CapturedOn { get; set; } = DateTime.Now;

    // Shot Planning Information

    public int ShotNumber { get; set; }

    public int TotalShots { get; set; }

    public double PipeLength { get; set; }

    public double ShotSize { get; set; }

    public double Overlap { get; set; }

    public double ShotStartPosition { get; set; }

    public double ShotEndPosition { get; set; }

    public string ShotPosition =>
        $"{ShotStartPosition:0}-{ShotEndPosition:0} mm";

    // Review Information

    public string ReviewStatus { get; set; } = "PENDING";

    public string ReviewedBy { get; set; } = "";

    public DateTime? ReviewedOn { get; set; }
}