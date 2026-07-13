using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class DetectorViewModel : INotifyPropertyChanged
{
    private readonly DetectorService _detectorService = new();

    private DetectorModel? _selectedDetector;

    public ObservableCollection<DetectorModel> Detectors { get; } = new();

    public DetectorModel? SelectedDetector
    {
        get => _selectedDetector;
        set
        {
            _selectedDetector = value;
            OnPropertyChanged();
        }
    }

    public DetectorViewModel()
    {
        Load();
    }

    public void Load()
    {
        Detectors.Clear();

        foreach (var detector in _detectorService.GetAll())
        {
            Detectors.Add(detector);
        }
    }

    public void AddDemoDetector()
    {
        var detector = new DetectorModel
        {
            Name = "DR Detector 01",
            Manufacturer = "Varex",
            Model = "2520DX",
            SerialNumber = $"DET-{DateTime.Now:yyyyMMddHHmmss}",
            IpAddress = "192.168.0.100",
            Port = 5000,
            Status = "Offline"
        };

        _detectorService.Save(detector);

        Load();
    }

    public void ConnectSelected()
    {
        if (SelectedDetector == null)
            return;

        _detectorService.UpdateConnectionStatus(
            SelectedDetector.Id,
            true);

        Load();
    }

    public void DisconnectSelected()
    {
        if (SelectedDetector == null)
            return;

        _detectorService.UpdateConnectionStatus(
            SelectedDetector.Id,
            false);

        Load();
    }

    public void DeleteSelected()
    {
        if (SelectedDetector == null)
            return;

        _detectorService.Delete(SelectedDetector.Id);

        Load();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}