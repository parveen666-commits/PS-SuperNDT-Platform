using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using PS.SuperNDT.UI.Commands;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class LicenseViewModel : INotifyPropertyChanged
{
    private readonly LicenseService _licenseService = new();

    private string _licenseKey = string.Empty;
    private string _licenseStatus = string.Empty;
    private int _remainingDays;

    public RelayCommand ActivateCommand { get; }

    public string LicenseKey
    {
        get => _licenseKey;
        set
        {
            _licenseKey = value;
            OnPropertyChanged();
        }
    }

    public string LicenseStatus
    {
        get => _licenseStatus;
        set
        {
            _licenseStatus = value;
            OnPropertyChanged();
        }
    }

    public int RemainingDays
    {
        get => _remainingDays;
        set
        {
            _remainingDays = value;
            OnPropertyChanged();
        }
    }

    public LicenseViewModel()
    {
        LoadLicense();

        ActivateCommand =
            new RelayCommand(_ => ActivateLicense());
    }

    private void LoadLicense()
    {
        var license =
            _licenseService.GetLicense();

        LicenseKey =
            license.LicenseKey;

        RemainingDays =
            _licenseService.RemainingDays();

        LicenseStatus =
            license.IsActivated
                ? "Activated"
                : "Trial Mode";
    }

    private void ActivateLicense()
    {
        if (string.IsNullOrWhiteSpace(LicenseKey))
        {
            MessageBox.Show(
                "Please enter a license key.",
                "PS SuperNDT",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        var license =
            _licenseService.GetLicense();

        license.IsActivated = true;
        license.LicenseKey = LicenseKey;

        _licenseService.SaveLicense(license);

        LicenseStatus = "Activated";
        RemainingDays = int.MaxValue;

        MessageBox.Show(
            "License activated successfully.",
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