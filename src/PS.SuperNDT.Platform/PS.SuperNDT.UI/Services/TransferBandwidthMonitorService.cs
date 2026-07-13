using System;
using System.Collections.ObjectModel;
using System.Linq;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class TransferBandwidthMonitorService
{
    public ObservableCollection<TransferBandwidthSnapshotModel> Snapshots { get; } = new();

    public void RecordSnapshot(
        double currentMbps,
        long bytesTransferred,
        int activeTransfers,
        int queueDepth,
        string networkStatus)
    {
        var averageMbps = currentMbps;
        var peakMbps = currentMbps;

        if (Snapshots.Count > 0)
        {
            averageMbps =
                Snapshots.Average(x => x.CurrentMbps);

            peakMbps =
                Math.Max(
                    currentMbps,
                    Snapshots.Max(x => x.PeakMbps));
        }

        Snapshots.Insert(0, new TransferBandwidthSnapshotModel
        {
            Id = Guid.NewGuid(),
            Timestamp = DateTime.Now,
            CurrentMbps = currentMbps,
            AverageMbps = averageMbps,
            PeakMbps = peakMbps,
            BytesTransferred = bytesTransferred,
            ActiveTransfers = activeTransfers,
            QueueDepth = queueDepth,
            NetworkStatus = networkStatus,
            Remarks = string.Empty
        });

        while (Snapshots.Count > 500)
        {
            Snapshots.RemoveAt(Snapshots.Count - 1);
        }
    }

    public TransferBandwidthSnapshotModel? GetLatest()
    {
        return Snapshots.FirstOrDefault();
    }

    public void Clear()
    {
        Snapshots.Clear();
    }
}