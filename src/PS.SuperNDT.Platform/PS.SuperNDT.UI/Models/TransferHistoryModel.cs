using System;

namespace PS.SuperNDT.UI.Models;

public sealed class TransferHistoryModel
{
    public Guid Id { get; set; }

    public string JobNumber { get; set; } = string.Empty;

    public string InspectionNumber { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public string SourcePath { get; set; } = string.Empty;

    public string DestinationPath { get; set; } = string.Empty;

    public DateTime TransferDateTime { get; set; }

    public TransferStatus Status { get; set; }

    public long FileSizeBytes { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string Remarks { get; set; } = string.Empty;
}