using System;

namespace PS.SuperNDT.UI.Models;

public sealed class TransferQueueItemMetricsModel
{
    public Guid TransferId { get; set; }

    public string JobNumber { get; set; } = string.Empty;

    public string InspectionNumber { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public long FileSizeBytes { get; set; }

    public DateTime QueuedOn { get; set; }

    public DateTime? StartedOn { get; set; }

    public DateTime? CompletedOn { get; set; }

    public double TransferDurationSeconds { get; set; }

    public double TransferSpeedMbps { get; set; }

    public TransferStatus Status { get; set; }

    public int RetryCount { get; set; }

    public string LastError { get; set; } = string.Empty;
}