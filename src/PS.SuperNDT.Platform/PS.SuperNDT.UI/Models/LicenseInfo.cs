using System;

namespace PS.SuperNDT.UI.Models;

public sealed class LicenseInfo
{
    public bool IsActivated { get; set; }

    public string LicenseKey { get; set; } = string.Empty;

    public DateTime InstallDate { get; set; }

    public DateTime ExpiryDate { get; set; }
}