using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class DashboardViewModel : INotifyPropertyChanged
{
    private readonly DispatcherTimer _timer;

    private string _currentJob = "No Active Job";
    private int _totalImages;
    private int _totalJobs;
    private int _openJobs;
    private int _closedJobs;

    public string CurrentJob
    {
        get => _currentJob;
        set
        {
            _currentJob = value;
            OnPropertyChanged();
        }
    }

    public int TotalImages
    {
        get => _totalImages;
        set
        {
            _totalImages = value;
            OnPropertyChanged();
        }
    }

    public int TotalJobs
    {
        get => _totalJobs;
        set
        {
            _totalJobs = value;
            OnPropertyChanged();
        }
    }

    public int OpenJobs
    {
        get => _openJobs;
        set
        {
            _openJobs = value;
            OnPropertyChanged();
        }
    }

    public int ClosedJobs
    {
        get => _closedJobs;
        set
        {
            _closedJobs = value;
            OnPropertyChanged();
        }
    }

    public DashboardViewModel()
    {
        Refresh();

        _timer = new DispatcherTimer
        {
            Interval = System.TimeSpan.FromSeconds(2)
        };

        _timer.Tick += (_, _) => Refresh();
        _timer.Start();
    }

    private void Refresh()
    {
        var currentJob =
            CurrentJobService.Instance.CurrentJob;

        CurrentJob =
            currentJob?.JobNumber ??
            "No Active Job";

        var imageService = new ImageService();

        TotalImages =
            imageService.GetTotalImageCount();

        var jobService = new JobService();

        var jobs =
            jobService.GetAll();

        TotalJobs =
            jobs.Count;

        OpenJobs =
            jobs.Count(x => !x.IsClosed);

        ClosedJobs =
            jobs.Count(x => x.IsClosed);
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