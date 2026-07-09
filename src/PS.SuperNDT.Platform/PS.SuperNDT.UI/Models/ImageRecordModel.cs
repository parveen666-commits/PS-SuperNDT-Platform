using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ImageRecordModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid JobId { get; set; }

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
}