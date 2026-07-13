using System;

namespace PS.SuperNDT.UI.Models;

public sealed class InspectionTransferModel
{
    public Guid Id { get; set; }

    public string JobNumber { get; set; } = string.Empty;

    public string InspectionNumber { get; set; } = string.Empty;

    public string ImagePath { get; set; } = string.Empty;

    public string Destination { get; set; } = string.Empty;

    public DateTime CreatedOn { get; set; }

    public DateTime? StartedOn { get; set; }

    public DateTime? CompletedOn { get; set; }

    public TransferStatus Status { get; set; }

    public string ErrorMessage { get; set; } = string.Empty;

    public long FileSizeBytes { get; set; }

    public bool AutoTransfer { get; set; }

    public string CreatedBy { get; set; } = string.Empty;
}