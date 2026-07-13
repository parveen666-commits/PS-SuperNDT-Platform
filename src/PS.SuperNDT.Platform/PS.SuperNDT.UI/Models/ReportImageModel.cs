using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportImageModel
{
    public Guid Id { get; set; }

    public Guid ReportId { get; set; }

    public string ImageName { get; set; } = string.Empty;

    public int SequenceNumber { get; set; }

    public string FilePath { get; set; } = string.Empty;

    public string ThumbnailPath { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public string ImageType { get; set; } = string.Empty;

    public long FileSizeBytes { get; set; }

    public DateTime AddedOn { get; set; }

    public string AddedBy { get; set; } = string.Empty;

    public DateTime? CapturedOn { get; set; }

    public string CapturedBy { get; set; } = string.Empty;

    public string Remarks { get; set; } = string.Empty;

    public bool IsSelected { get; set; }
}