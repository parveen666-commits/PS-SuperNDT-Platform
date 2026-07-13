using System;
using System.Linq;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class TransferPerformanceService
{
    private readonly InspectionTransferService _transferService;

    public TransferPerformanceService(
        InspectionTransferService transferService)
    {
        _transferService = transferService;
    }

    public TransferPerformanceModel GetPerformance()
    {
        var sentItems = _transferService.Queue
            .Where(x => x.Status == TransferStatus.Sent)
            .ToList();

        var failedItems = _transferService.Queue
            .Where(x => x.Status == TransferStatus.Failed)
            .ToList();

        var performance = new TransferPerformanceModel
        {
            Timestamp = DateTime.Now,
            TransfersCompleted = sentItems.Count,
            TransfersFailed = failedItems.Count,
            BytesTransferred = sentItems.Sum(x => x.FileSizeBytes)
        };

        var durations = sentItems
            .Where(x => x.StartedOn.HasValue && x.CompletedOn.HasValue)
            .Select(x => (x.CompletedOn!.Value - x.StartedOn!.Value).TotalSeconds)
            .ToList();

        if (durations.Count > 0)
        {
            performance.AverageTransferTimeSeconds = durations.Average();
            performance.FastestTransferSeconds = durations.Min();
            performance.SlowestTransferSeconds = durations.Max();
        }

        var total = performance.TransfersCompleted + performance.TransfersFailed;

        if (total > 0)
        {
            performance.SuccessRate =
                (double)performance.TransfersCompleted / total * 100.0;

            performance.FailureRate =
                (double)performance.TransfersFailed / total * 100.0;
        }

        performance.BestPerformancePeriod = "Current Session";
        performance.WorstPerformancePeriod = "Current Session";

        return performance;
    }
}