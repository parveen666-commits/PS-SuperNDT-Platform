using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Commands;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class AcquisitionViewModel : INotifyPropertyChanged
{
    private string _detectorStatus = "Offline";
    private string _connectionStatus = "Disconnected";
    private string _currentJob = "No Active Job";

    private int _frameNumber;
    private double _kv = 120;
    private double _ma = 5;
    private double _exposureTime = 2;


    public RelayCommand ConnectCommand { get; }

    public RelayCommand CaptureCommand { get; }


    public string DetectorStatus
    {
        get => _detectorStatus;
        set
        {
            _detectorStatus = value;
            OnPropertyChanged();
        }
    }


    public string ConnectionStatus
    {
        get => _connectionStatus;
        set
        {
            _connectionStatus = value;
            OnPropertyChanged();
        }
    }


    public string CurrentJob
    {
        get => _currentJob;
        set
        {
            _currentJob = value;
            OnPropertyChanged();
        }
    }


    public int FrameNumber
    {
        get => _frameNumber;
        set
        {
            _frameNumber = value;
            OnPropertyChanged();
        }
    }


    public double KV
    {
        get => _kv;
        set
        {
            _kv = value;
            OnPropertyChanged();
        }
    }


    public double MA
    {
        get => _ma;
        set
        {
            _ma = value;
            OnPropertyChanged();
        }
    }


    public double ExposureTime
    {
        get => _exposureTime;
        set
        {
            _exposureTime = value;
            OnPropertyChanged();
        }
    }



    public AcquisitionViewModel()
    {
        UpdateCurrentJob();

        CurrentJobService.Instance.CurrentJobChanged += (_, _) =>
        {
            UpdateCurrentJob();
        };


        ConnectCommand = new RelayCommand(
            _ => ConnectDetector());


        CaptureCommand = new RelayCommand(
            _ => CaptureImage());
    }



    private void UpdateCurrentJob()
    {
        if (CurrentJobService.Instance.CurrentJob != null)
        {
            CurrentJob =
                CurrentJobService.Instance
                .CurrentJob!
                .JobNumber;
        }
        else
        {
            CurrentJob = "No Active Job";
        }
    }



    private void ConnectDetector()
    {
        DetectorStatus = "Ready";
        ConnectionStatus = "Connected";
    }



    private void CaptureImage()
    {
        FrameNumber++;

        // Capture workflow will connect here:
        // Detector SDK
        // Image acquisition
        // Database save
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