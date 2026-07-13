using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportPackageModel
{
    public Guid Id { get; set; }

    public string PackageNumber { get; set; } = string.Empty;

    public string ReportNumber { get; set; } = string.Empty;

    public string JobNumber { get; set; } = string.Empty;

    public string PackagePath { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public long PackageSizeBytes { get; set; }

    public int FileCount { get; set; }

    public bool IsCompressed { get; set; }

    public bool IsVerified { get; set; }

    public DateTime CreatedOn { get; set; }

    public string CreatedBy { get; set; } = string.Empty;

    public string Remarks { get; set; } = string.Empty;
}