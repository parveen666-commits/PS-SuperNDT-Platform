using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Commands;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class AcquisitionViewModel : INotifyPropertyChanged
{
    private readonly ImageService _imageService = new();

    private string _detectorStatus = "Offline";
    private string _connectionStatus = "Disconnected";
    private string _currentJob = "No Active Job";
    private string _acquisitionStatus = "Ready";

    private int _frameNumber;
    private double _kv = 120;
    private double _ma = 5;
    private double _exposureTime = 2;

    private ImageRecordModel? _capturedImage;

    public RelayCommand ConnectCommand { get; }

    public RelayCommand CaptureCommand { get; }

    public RelayCommand SaveCommand { get; }

    public RelayCommand RetakeCommand { get; }

    public RelayCommand WindowLevelCommand { get; }

    public RelayCommand ZoomCommand { get; }

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

    public string AcquisitionStatus
    {
        get => _acquisitionStatus;
        set
        {
            _acquisitionStatus = value;
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

    public bool IsConnected =>
        ConnectionStatus == "Connected";

    public bool HasCapturedImage =>
        _capturedImage != null;

    public AcquisitionViewModel()
    {
        UpdateCurrentJob();

        CurrentJobService.Instance.CurrentJobChanged +=
            (_, _) =>
            {
                UpdateCurrentJob();
            };

        ConnectCommand =
            new RelayCommand(
                _ => ConnectDetector());

        CaptureCommand =
            new RelayCommand(
                _ => CaptureImage());

        SaveCommand =
            new RelayCommand(
                _ => SaveImage());

        RetakeCommand =
            new RelayCommand(
                _ => RetakeImage());

        WindowLevelCommand =
            new RelayCommand(
                _ => ApplyWindowLevel());

        ZoomCommand =
            new RelayCommand(
                _ => ApplyZoom());
    }

    private void UpdateCurrentJob()
    {
        var job =
            CurrentJobService.Instance.CurrentJob;

        if (job != null)
        {
            CurrentJob =
                job.JobNumber;

            AcquisitionStatus =
                "Ready";
        }
        else
        {
            CurrentJob =
                "No Active Job";

            AcquisitionStatus =
                "Open a job before acquisition";
        }
    }

    private void ConnectDetector()
    {
        DetectorStatus = "Connecting...";
        ConnectionStatus = "Connecting...";
        AcquisitionStatus = "Connecting to detector...";

        // Virtual detector connection.
        // Real detector communication can be connected
        // here later without changing the UI workflow.
        ConnectionStatus = "Connected";
        DetectorStatus = "Ready";
        AcquisitionStatus = "Detector connected";

        OnPropertyChanged(nameof(IsConnected));
    }

    private void CaptureImage()
    {
        var job =
            CurrentJobService.Instance.CurrentJob;

        if (job == null)
        {
            AcquisitionStatus =
                "No active job. Open or create a job first.";

            return;
        }

        if (!IsConnected)
        {
            AcquisitionStatus =
                "Detector is not connected.";

            return;
        }

        if (KV <= 0 ||
            MA <= 0 ||
            ExposureTime <= 0)
        {
            AcquisitionStatus =
                "Exposure values must be greater than zero.";

            return;
        }

        FrameNumber++;

        _capturedImage =
            new ImageRecordModel
            {
                JobId = job.Id,
                JobNumber = job.JobNumber,
                Operator = job.Operator,

                FrameNumber = FrameNumber,

                FileName =
                    $"IMG_{FrameNumber:0000}.ndt",

                FilePath =
                    string.Empty,

                DetectorName =
                    "Virtual Detector",

                KV = KV,
                MA = MA,
                ExposureTime = ExposureTime,

                ImageWidth = 2048,
                ImageHeight = 2048,
                BitDepth = 16,

                CapturedOn = DateTime.Now
            };

        AcquisitionStatus =
            $"Frame {FrameNumber:0000} captured";

        OnPropertyChanged(nameof(HasCapturedImage));

        ImageViewerService.Instance.OpenImage(
            _capturedImage);
    }

    private void SaveImage()
    {
        if (_capturedImage == null)
        {
            AcquisitionStatus =
                "No captured image available to save.";

            return;
        }

        _imageService.Save(
            _capturedImage);

        AcquisitionStatus =
            $"Image {_capturedImage.FileName} saved";

        _capturedImage = null;

        OnPropertyChanged(nameof(HasCapturedImage));
    }

    private void RetakeImage()
    {
        if (_capturedImage == null)
        {
            AcquisitionStatus =
                "No captured image to retake.";

            return;
        }

        _capturedImage = null;

        if (FrameNumber > 0)
        {
            FrameNumber--;
        }

        AcquisitionStatus =
            "Ready for retake";

        OnPropertyChanged(nameof(HasCapturedImage));
    }

    private void ApplyWindowLevel()
    {
        if (_capturedImage == null)
        {
            AcquisitionStatus =
                "Capture an image before adjusting Window / Level.";

            return;
        }

        AcquisitionStatus =
            "Window / Level adjustment ready";
    }

    private void ApplyZoom()
    {
        if (_capturedImage == null)
        {
            AcquisitionStatus =
                "Capture an image before using Zoom.";

            return;
        }

        AcquisitionStatus =
            "Zoom adjustment ready";
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(
        [CallerMemberName]
        string propertyName = "")
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(
                propertyName));
    }
}