using System;
using System.IO;
using System.Linq;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class TransferHealthMonitorService
{
    private readonly InspectionTransferService _transferService;
    private readonly TransferEngineSettingsService _settingsService;

    public TransferHealthMonitorService(
        InspectionTransferService transferService,
        TransferEngineSettingsService settingsService)
    {
        _transferService = transferService;
        _settingsService = settingsService;
    }

    public TransferHealthStatusModel GetStatus()
    {
        var settings = _settingsService.GetSettings();

        var status = new TransferHealthStatusModel
        {
            LastUpdated = DateTime.Now,
            TransferEngineRunning = true,
            TransferFolderAccessible = Directory.Exists(settings.TransferFolder),
            ArchiveFolderAccessible = Directory.Exists(settings.ArchiveFolder),
            ReviewStationReachable =
                !string.IsNullOrWhiteSpace(settings.ReviewStationAddress),
            DatabaseAvailable = true,
            DetectorConnected = true,
            PlcConnected = true,
            PendingTransfers = _transferService.Queue.Count(
                x => x.Status == TransferStatus.Pending),
            FailedTransfers = _transferService.Queue.Count(
                x => x.Status == TransferStatus.Failed)
        };

        try
        {
            var drive =
                DriveInfo.GetDrives()
                    .FirstOrDefault(x => x.IsReady);

            if (drive is not null)
            {
                status.AvailableDiskSpaceMb =
                    drive.AvailableFreeSpace / 1024 / 1024;
            }
        }
        catch
        {
            status.AvailableDiskSpaceMb = 0;
        }

        status.OverallStatus =
            status.PendingTransfers == 0 &&
            status.FailedTransfers == 0
                ? "Healthy"
                : "Warning";

        status.Message =
            status.OverallStatus == "Healthy"
                ? "Transfer Engine Operating Normally"
                : "Attention Required";

        return status;
    }
}