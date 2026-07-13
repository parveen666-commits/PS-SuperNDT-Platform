using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class TransferEngineSettingsService
{
    private readonly TransferEngineSettings _settings;

    public TransferEngineSettingsService()
    {
        _settings = new TransferEngineSettings
        {
            AutoTransferEnabled = false,
            RetryFailedTransfers = true,
            RetryCount = 3,
            RetryDelaySeconds = 10,
            TransferFolder = @"C:\PS-SuperNDT\Transfer",
            ArchiveFolder = @"C:\PS-SuperNDT\Archive",
            ReviewStationName = "Review Station 01",
            ReviewStationAddress = @"\\REVIEW-PC\PSReview",
            DeleteSourceAfterTransfer = false,
            CreateTransferLog = true,
            MinimumFreeDiskSpaceMb = 1024
        };
    }

    public TransferEngineSettings GetSettings()
    {
        return _settings;
    }

    public void SaveSettings(TransferEngineSettings settings)
    {
        _settings.AutoTransferEnabled = settings.AutoTransferEnabled;
        _settings.RetryFailedTransfers = settings.RetryFailedTransfers;
        _settings.RetryCount = settings.RetryCount;
        _settings.RetryDelaySeconds = settings.RetryDelaySeconds;
        _settings.TransferFolder = settings.TransferFolder;
        _settings.ArchiveFolder = settings.ArchiveFolder;
        _settings.ReviewStationName = settings.ReviewStationName;
        _settings.ReviewStationAddress = settings.ReviewStationAddress;
        _settings.DeleteSourceAfterTransfer = settings.DeleteSourceAfterTransfer;
        _settings.CreateTransferLog = settings.CreateTransferLog;
        _settings.MinimumFreeDiskSpaceMb = settings.MinimumFreeDiskSpaceMb;
    }
}