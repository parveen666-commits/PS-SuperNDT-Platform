using System;

namespace PS.SuperNDT.UI.Models;

public sealed class DetectorModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string Manufacturer { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public string SerialNumber { get; set; } = string.Empty;

    public string IpAddress { get; set; } = string.Empty;

    public int Port { get; set; }

    public bool IsConnected { get; set; }

    public string FirmwareVersion { get; set; } = string.Empty;

    public string DetectorType { get; set; } = "DR";

    public double PixelPitch { get; set; }

    public int ImageWidth { get; set; }

    public int ImageHeight { get; set; }

    public double BatteryLevel { get; set; }

    public DateTime LastHeartbeat { get; set; } = DateTime.Now;

    public string Status { get; set; } = "Offline";

    public string Remarks { get; set; } = string.Empty;
}