using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class DashboardViewModel : INotifyPropertyChanged
{
    private readonly DispatcherTimer _timer;

    private string _currentTime = "";
    private string _detectorStatus = "Ready";
    private string _plcStatus = "Offline";
    private string _currentRecipe = "Default";
    private string _currentJob = "No Job";
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
        TotalShots = 0;
        RejectCount = 0;
        StoragePercent = 35;

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        _timer.Tick += (s, e) =>
        {
            CurrentTime = DateTime.Now.ToString(
                "dd-MMM-yyyy HH:mm:ss");
        };

        _timer.Start();
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