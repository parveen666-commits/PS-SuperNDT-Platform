namespace PS.SuperNDT.UI.Models;

public sealed class TransferEngineSettings
{
    public bool AutoTransferEnabled { get; set; }

    public bool RetryFailedTransfers { get; set; }

    public int RetryCount { get; set; }

    public int RetryDelaySeconds { get; set; }

    public string TransferFolder { get; set; } = string.Empty;

    public string ArchiveFolder { get; set; } = string.Empty;

    public string ReviewStationName { get; set; } = string.Empty;

    public string ReviewStationAddress { get; set; } = string.Empty;

    public bool DeleteSourceAfterTransfer { get; set; }

    public bool CreateTransferLog { get; set; }

    public long MinimumFreeDiskSpaceMb { get; set; } = 1024;
}