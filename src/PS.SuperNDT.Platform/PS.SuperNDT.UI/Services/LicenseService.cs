using System;
using System.IO;
using System.Text.Json;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class LicenseService
{
    private readonly string _licenseFile =
        Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "license.json");

    public LicenseInfo GetLicense()
    {
        try
        {
            if (!File.Exists(_licenseFile))
            {
                var trialLicense = new LicenseInfo
                {
                    IsActivated = false,
                    LicenseKey = string.Empty,
                    InstallDate = DateTime.Today,
                    ExpiryDate = DateTime.Today.AddDays(30)
                };

                SaveLicense(trialLicense);

                return trialLicense;
            }

            var json =
                File.ReadAllText(_licenseFile);

            var license =
                JsonSerializer.Deserialize<LicenseInfo>(json);

            return license ?? new LicenseInfo
            {
                InstallDate = DateTime.Today,
                ExpiryDate = DateTime.Today.AddDays(30)
            };
        }
        catch
        {
            return new LicenseInfo
            {
                InstallDate = DateTime.Today,
                ExpiryDate = DateTime.Today.AddDays(30)
            };
        }
    }

    public void SaveLicense(
        LicenseInfo license)
    {
        var json =
            JsonSerializer.Serialize(
                license,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

        File.WriteAllText(
            _licenseFile,
            json);
    }

    public bool IsLicenseValid()
    {
        var license = GetLicense();

        if (license.IsActivated)
            return true;

        return DateTime.Today <=
               license.ExpiryDate.Date;
    }

    public int RemainingDays()
    {
        var license = GetLicense();

        if (license.IsActivated)
            return int.MaxValue;

        return Math.Max(
            0,
            (license.ExpiryDate.Date -
             DateTime.Today).Days);
    }
}