namespace PS.SuperNDT.UI.Models;

public sealed class TransferQueueSummaryModel
{
    public int TotalItems { get; set; }

    public int PendingItems { get; set; }

    public int QueuedItems { get; set; }

    public int SendingItems { get; set; }

    public int SentItems { get; set; }

    public int FailedItems { get; set; }

    public int CancelledItems { get; set; }

    public long TotalQueueSizeBytes { get; set; }

    public long CompletedTransferSizeBytes { get; set; }

    public string OldestPendingInspection { get; set; } = string.Empty;

    public string LatestTransferredInspection { get; set; } = string.Empty;
}