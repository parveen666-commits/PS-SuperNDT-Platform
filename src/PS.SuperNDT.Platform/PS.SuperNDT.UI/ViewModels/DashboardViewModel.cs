using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class DashboardViewModel : INotifyPropertyChanged
{
    private readonly DispatcherTimer _timer;
    private readonly ImageService _imageService = new();

    private string _currentTime = "";
    private string _detectorStatus = "Ready";
    private string _plcStatus = "Offline";
    private string _currentRecipe = "Default";
    private string _currentJob = "No Active Job";
    private int _totalShots;
    private int _rejectCount;
    private double _storagePercent;


    public string CurrentTime
    {
        get => _currentTime;
        set
        {
            _currentTime = value;
            OnPropertyChanged();
        }
    }


    public string DetectorStatus
    {
        get => _detectorStatus;
        set
        {
            _detectorStatus = value;
            OnPropertyChanged();
        }
    }


    public string PLCStatus
    {
        get => _plcStatus;
        set
        {
            _plcStatus = value;
            OnPropertyChanged();
        }
    }


    public string CurrentRecipe
    {
        get => _currentRecipe;
        set
        {
            _currentRecipe = value;
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


    public int TotalShots
    {
        get => _totalShots;
        set
        {
            _totalShots = value;
            OnPropertyChanged();
        }
    }


    public int RejectCount
    {
        get => _rejectCount;
        set
        {
            _rejectCount = value;
            OnPropertyChanged();
        }
    }


    public double StoragePercent
    {
        get => _storagePercent;
        set
        {
            _storagePercent = value;
            OnPropertyChanged();
        }
    }


    public DashboardViewModel()
    {
        StoragePercent = 35;

        UpdateCurrentJob();
        UpdateShotCount();

        CurrentJobService.Instance.CurrentJobChanged += (_, _) =>
        {
            UpdateCurrentJob();
            UpdateShotCount();
        };


        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };


        _timer.Tick += (_, _) =>
        {
            CurrentTime =
                DateTime.Now.ToString(
                    "dd-MMM-yyyy HH:mm:ss");

            UpdateShotCount();
        };


        _timer.Start();
    }


    private void UpdateCurrentJob()
    {
        if (CurrentJobService.Instance.HasCurrentJob)
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


    private void UpdateShotCount()
    {
        var job =
            CurrentJobService.Instance.CurrentJob;

        if (job == null)
        {
            TotalShots = 0;
            return;
        }

        TotalShots =
            _imageService.GetImageCount(job.Id);
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