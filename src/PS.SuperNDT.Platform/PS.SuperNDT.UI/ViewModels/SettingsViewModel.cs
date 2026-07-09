using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using PS.SuperNDT.UI.Commands;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class SettingsViewModel : INotifyPropertyChanged
{
    private readonly SettingsService _settingsService = new();

    private string _databasePath = "";
    private string _storagePath = "";
    private bool _autoConnectDetector;
    private bool _enableImagePreview;
    private bool _autoConnectPlc;
    private string _plcIpAddress = "";
    private bool _darkTheme;

    public RelayCommand SaveCommand { get; }

    public string DatabasePath
    {
        get => _databasePath;
        set
        {
            _databasePath = value;
            OnPropertyChanged();
        }
    }

    public string StoragePath
    {
        get => _storagePath;
        set
        {
            _storagePath = value;
            OnPropertyChanged();
        }
    }

    public bool AutoConnectDetector
    {
        get => _autoConnectDetector;
        set
        {
            _autoConnectDetector = value;
            OnPropertyChanged();
        }
    }

    public bool EnableImagePreview
    {
        get => _enableImagePreview;
        set
        {
            _enableImagePreview = value;
            OnPropertyChanged();
        }
    }

    public bool AutoConnectPlc
    {
        get => _autoConnectPlc;
        set
        {
            _autoConnectPlc = value;
            OnPropertyChanged();
        }
    }

    public string PlcIpAddress
    {
        get => _plcIpAddress;
        set
        {
            _plcIpAddress = value;
            OnPropertyChanged();
        }
    }

    public bool DarkTheme
    {
        get => _darkTheme;
        set
        {
            _darkTheme = value;
            OnPropertyChanged();
        }
    }

    public SettingsViewModel()
    {
        LoadSettings();

        SaveCommand =
            new RelayCommand(_ => SaveSettings());
    }

    private void LoadSettings()
    {
        var settings =
            _settingsService.Load();

        DatabasePath = settings.DatabasePath;
        StoragePath = settings.StoragePath;
        AutoConnectDetector = settings.AutoConnectDetector;
        EnableImagePreview = settings.EnableImagePreview;
        AutoConnectPlc = settings.AutoConnectPlc;
        PlcIpAddress = settings.PlcIpAddress;
        DarkTheme = settings.DarkTheme;
    }

    private void SaveSettings()
    {
        var settings = new ApplicationSettings
        {
            DatabasePath = DatabasePath,
            StoragePath = StoragePath,
            AutoConnectDetector = AutoConnectDetector,
            EnableImagePreview = EnableImagePreview,
            AutoConnectPlc = AutoConnectPlc,
            PlcIpAddress = PlcIpAddress,
            DarkTheme = DarkTheme
        };

        _settingsService.Save(settings);

        MessageBox.Show(
            "Settings saved successfully.",
            "PS SuperNDT",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(
        [CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}