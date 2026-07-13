namespace PS.SuperNDT.UI.Models;

public sealed class TransferEngineStatistics
{
    public int PendingCount { get; set; }

    public int QueuedCount { get; set; }

    public int SendingCount { get; set; }

    public int SentCount { get; set; }

    public int FailedCount { get; set; }

    public int CancelledCount { get; set; }

    public int TotalCount { get; set; }

    public long TotalBytesTransferred { get; set; }

    public string LastTransferTime { get; set; } = "--";

    public string LastTransferJob { get; set; } = "--";

    public string LastTransferInspection { get; set; } = "--";
}