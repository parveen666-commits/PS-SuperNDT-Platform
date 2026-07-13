using System;

namespace PS.SuperNDT.UI.Models;

public sealed class TransferBandwidthSnapshotModel
{
    public Guid Id { get; set; }

    public DateTime Timestamp { get; set; }

    public double CurrentMbps { get; set; }

    public double AverageMbps { get; set; }

    public double PeakMbps { get; set; }

    public long BytesTransferred { get; set; }

    public int ActiveTransfers { get; set; }

    public int QueueDepth { get; set; }

    public string NetworkStatus { get; set; } = string.Empty;

    public string Remarks { get; set; } = string.Empty;
}