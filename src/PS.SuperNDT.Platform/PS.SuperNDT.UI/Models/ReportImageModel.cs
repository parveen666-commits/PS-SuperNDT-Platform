using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportImageModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ReportId { get; set; }

    public string ImageName { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public string ImageType { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public int SequenceNumber { get; set; }

    public DateTime AddedOn { get; set; } = DateTime.Now;

    public string AddedBy { get; set; } = string.Empty;
}