using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ReportsViewModel : INotifyPropertyChanged
{
    private readonly JobService _jobService = new();
    private readonly ImageService _imageService = new();

    public ObservableCollection<JobModel> Jobs { get; } = new();

    private int _totalJobs;
    private int _openJobs;
    private int _closedJobs;
    private int _totalImages;

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

    public int TotalImages
    {
        get => _totalImages;
        set
        {
            _totalImages = value;
            OnPropertyChanged();
        }
    }

    public ReportsViewModel()
    {
        LoadData();
    }

    private void LoadData()
    {
        var jobs = _jobService.GetAll();

        Jobs.Clear();

        foreach (var job in jobs)
        {
            Jobs.Add(job);
        }

        TotalJobs = jobs.Count;
        OpenJobs = jobs.Count(x => !x.IsClosed);
        ClosedJobs = jobs.Count(x => x.IsClosed);

        TotalImages =
            _imageService.GetTotalImageCount();
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