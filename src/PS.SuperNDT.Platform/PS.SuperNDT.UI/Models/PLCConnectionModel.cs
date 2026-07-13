using System;

namespace PS.SuperNDT.UI.Models;

public sealed class PLCConnectionModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = string.Empty;

    public string PlcType { get; set; } = "Siemens S7-200 Smart";

    public string IpAddress { get; set; } = "192.168.0.1";

    public int Rack { get; set; }

    public int Slot { get; set; } = 1;

    public int Port { get; set; } = 102;

    public bool IsConnected { get; set; }

    public string Status { get; set; } = "Offline";

    public string FirmwareVersion { get; set; } = string.Empty;

    public DateTime LastHeartbeat { get; set; } = DateTime.Now;

    public int ReadCount { get; set; }

    public int WriteCount { get; set; }

    public int ErrorCount { get; set; }

    public string Remarks { get; set; } = string.Empty;
}