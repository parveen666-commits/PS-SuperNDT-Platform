using System;
using System.Collections.ObjectModel;
using System.IO;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class TransferQueueItemMetricsService
{
    public ObservableCollection<TransferQueueItemMetricsModel> Metrics { get; } = new();

    public void Register(
        InspectionTransferModel transfer)
    {
        var duration = 0.0;

        if (transfer.StartedOn.HasValue &&
            transfer.CompletedOn.HasValue)
        {
            duration =
                (transfer.CompletedOn.Value -
                 transfer.StartedOn.Value).TotalSeconds;
        }

        var speedMbps = 0.0;

        if (duration > 0)
        {
            speedMbps =
                (transfer.FileSizeBytes / 1024d / 1024d) /
                duration;
        }

        Metrics.Insert(0, new TransferQueueItemMetricsModel
        {
            TransferId = transfer.Id,
            JobNumber = transfer.JobNumber,
            InspectionNumber = transfer.InspectionNumber,
            FileName = Path.GetFileName(transfer.ImagePath),
            FileSizeBytes = transfer.FileSizeBytes,
            QueuedOn = transfer.CreatedOn,
            StartedOn = transfer.StartedOn,
            CompletedOn = transfer.CompletedOn,
            TransferDurationSeconds = duration,
            TransferSpeedMbps = speedMbps,
            Status = transfer.Status,
            RetryCount = 0,
            LastError = transfer.ErrorMessage
        });
    }

    public void Clear()
    {
        Metrics.Clear();
    }

    public int Count()
    {
        return Metrics.Count;
    }
}