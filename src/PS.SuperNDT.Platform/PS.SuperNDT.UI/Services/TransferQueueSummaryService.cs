using System.Linq;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class TransferQueueSummaryService
{
    private readonly InspectionTransferService _transferService;

    public TransferQueueSummaryService(
        InspectionTransferService transferService)
    {
        _transferService = transferService;
    }

    public TransferQueueSummaryModel GetSummary()
    {
        var items = _transferService.Queue.ToList();

        var summary = new TransferQueueSummaryModel
        {
            TotalItems = items.Count,
            PendingItems = items.Count(x => x.Status == TransferStatus.Pending),
            QueuedItems = items.Count(x => x.Status == TransferStatus.Queued),
            SendingItems = items.Count(x => x.Status == TransferStatus.Sending),
            SentItems = items.Count(x => x.Status == TransferStatus.Sent),
            FailedItems = items.Count(x => x.Status == TransferStatus.Failed),
            CancelledItems = items.Count(x => x.Status == TransferStatus.Cancelled),

            TotalQueueSizeBytes = items.Sum(x => x.FileSizeBytes),

            CompletedTransferSizeBytes = items
                .Where(x => x.Status == TransferStatus.Sent)
                .Sum(x => x.FileSizeBytes)
        };

        var oldestPending = items
            .Where(x => x.Status == TransferStatus.Pending)
            .OrderBy(x => x.CreatedOn)
            .FirstOrDefault();

        if (oldestPending is not null)
        {
            summary.OldestPendingInspection =
                oldestPending.InspectionNumber;
        }

        var latestTransferred = items
            .Where(x => x.Status == TransferStatus.Sent)
            .OrderByDescending(x => x.CompletedOn)
            .FirstOrDefault();

        if (latestTransferred is not null)
        {
            summary.LatestTransferredInspection =
                latestTransferred.InspectionNumber;
        }

        return summary;
    }
}