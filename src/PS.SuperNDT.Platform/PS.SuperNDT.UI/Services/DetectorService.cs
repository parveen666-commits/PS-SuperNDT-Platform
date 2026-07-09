using System;

namespace PS.SuperNDT.UI.Services;

public sealed class DetectorService
{
    private static readonly Lazy<DetectorService> _instance =
        new(() => new DetectorService());

    public static DetectorService Instance => _instance.Value;

    private DetectorService()
    {
    }

    public bool IsConnected { get; private set; }

    public string DetectorName { get; private set; } = "No Detector";

    public string IpAddress { get; private set; } = "--";

    public event EventHandler? ConnectionStateChanged;

    public bool Connect(
        string detectorName,
        string ipAddress)
    {
        DetectorName = detectorName;
        IpAddress = ipAddress;
        IsConnected = true;

        ConnectionStateChanged?.Invoke(
            this,
            EventArgs.Empty);

        return true;
    }

    public void Disconnect()
    {
        IsConnected = false;

        DetectorName = "No Detector";
        IpAddress = "--";

        ConnectionStateChanged?.Invoke(
            this,
            EventArgs.Empty);
    }

    public string GetStatus()
    {
        return IsConnected
            ? "Connected"
            : "Offline";
    }
}