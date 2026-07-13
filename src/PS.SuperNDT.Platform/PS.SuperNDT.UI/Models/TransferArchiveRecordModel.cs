using System;

namespace PS.SuperNDT.UI.Models;

public sealed class TransferArchiveRecordModel
{
    public Guid Id { get; set; }

    public string PackageNumber { get; set; } = string.Empty;

    public string JobNumber { get; set; } = string.Empty;

    public string InspectionNumber { get; set; } = string.Empty;

    public string ArchivePath { get; set; } = string.Empty;

    public string OriginalPath { get; set; } = string.Empty;

    public DateTime ArchivedOn { get; set; }

    public string ArchivedBy { get; set; } = string.Empty;

    public long ArchiveSizeBytes { get; set; }

    public int FileCount { get; set; }

    public bool Verified { get; set; }

    public DateTime? VerifiedOn { get; set; }

    public string Remarks { get; set; } = string.Empty;
}