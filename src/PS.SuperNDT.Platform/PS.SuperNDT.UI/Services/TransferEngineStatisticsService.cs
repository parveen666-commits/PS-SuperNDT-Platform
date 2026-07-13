using System.Linq;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class TransferEngineStatisticsService
{
    private readonly InspectionTransferService _transferService;

    public TransferEngineStatisticsService(
        InspectionTransferService transferService)
    {
        _transferService = transferService;
    }

    public TransferEngineStatistics GetStatistics()
    {
        var items = _transferService.Queue.ToList();

        var statistics = new TransferEngineStatistics
        {
            PendingCount = items.Count(x => x.Status == TransferStatus.Pending),
            QueuedCount = items.Count(x => x.Status == TransferStatus.Queued),
            SendingCount = items.Count(x => x.Status == TransferStatus.Sending),
            SentCount = items.Count(x => x.Status == TransferStatus.Sent),
            FailedCount = items.Count(x => x.Status == TransferStatus.Failed),
            CancelledCount = items.Count(x => x.Status == TransferStatus.Cancelled),
            TotalCount = items.Count,
            TotalBytesTransferred = items
                .Where(x => x.Status == TransferStatus.Sent)
                .Sum(x => x.FileSizeBytes)
        };

        var lastTransfer = items
            .Where(x => x.Status == TransferStatus.Sent)
            .OrderByDescending(x => x.CompletedOn)
            .FirstOrDefault();

        if (lastTransfer is not null)
        {
            statistics.LastTransferTime =
                lastTransfer.CompletedOn?.ToString("dd-MMM-yyyy HH:mm:ss") ?? "--";

            statistics.LastTransferJob =
                lastTransfer.JobNumber;

            statistics.LastTransferInspection =
                lastTransfer.InspectionNumber;
        }

        return statistics;
    }
}