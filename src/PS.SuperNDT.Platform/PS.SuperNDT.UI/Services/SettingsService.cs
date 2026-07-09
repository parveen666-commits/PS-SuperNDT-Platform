using System;
using System.IO;
using System.Text.Json;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class SettingsService
{
    private readonly string _settingsFile =
        Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "settings.json");

    public ApplicationSettings Load()
    {
        try
        {
            if (!File.Exists(_settingsFile))
            {
                var defaultSettings =
                    new ApplicationSettings();

                Save(defaultSettings);

                return defaultSettings;
            }

            var json =
                File.ReadAllText(_settingsFile);

            var loadedSettings =
                JsonSerializer.Deserialize<ApplicationSettings>(json);

            return loadedSettings ??
                   new ApplicationSettings();
        }
        catch
        {
            return new ApplicationSettings();
        }
    }

    public void Save(ApplicationSettings settings)
    {
        var json =
            JsonSerializer.Serialize(
                settings,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });

        File.WriteAllText(
            _settingsFile,
            json);
    }
}