using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportArchiveModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ReportId { get; set; }

    public string ReportNumber { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public string FileHash { get; set; } = string.Empty;

    public DateTime ArchivedOn { get; set; } = DateTime.Now;

    public string ArchivedBy { get; set; } = string.Empty;

    public string Version { get; set; } = "1.0";

    public bool IsLocked { get; set; }
}              