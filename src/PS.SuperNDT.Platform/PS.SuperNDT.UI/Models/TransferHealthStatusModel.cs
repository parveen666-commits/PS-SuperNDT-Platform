using System;

namespace PS.SuperNDT.UI.Models;

public sealed class TransferHealthStatusModel
{
    public DateTime LastUpdated { get; set; }

    public bool TransferEngineRunning { get; set; }

    public bool ReviewStationReachable { get; set; }

    public bool TransferFolderAccessible { get; set; }

    public bool ArchiveFolderAccessible { get; set; }

    public bool DatabaseAvailable { get; set; }

    public bool DetectorConnected { get; set; }

    public bool PlcConnected { get; set; }

    public long AvailableDiskSpaceMb { get; set; }

    public int PendingTransfers { get; set; }

    public int FailedTransfers { get; set; }

    public string OverallStatus { get; set; } = "Unknown";

    public string Message { get; set; } = string.Empty;
}