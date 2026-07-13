using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportExportModel
{
    public Guid Id { get; set; }

    public Guid ReportId { get; set; }

    public string ExportFormat { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public long FileSizeBytes { get; set; }

    public DateTime ExportedOn { get; set; }

    public string ExportedBy { get; set; } = string.Empty;

    public bool IsSuccessful { get; set; }

    public string ErrorMessage { get; set; } = string.Empty;

    public string Remarks { get; set; } = string.Empty;
}