namespace PS.SuperNDT.UI.Models;

public sealed class ApplicationSettings
{
    public string DatabasePath { get; set; } =
        "PS_SuperNDT.db";

    public string StoragePath { get; set; } =
        "Images";

    public bool AutoConnectDetector { get; set; }

    public bool EnableImagePreview { get; set; } = true;

    public bool AutoConnectPlc { get; set; }

    public string PlcIpAddress { get; set; } =
        "192.168.0.1";

    public bool DarkTheme { get; set; } = true;
}