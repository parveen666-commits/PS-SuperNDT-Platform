using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PS.SuperNDT.UI.Commands;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ReportDashboardViewModel : INotifyPropertyChanged
{
    private readonly ReportStatisticsService _statisticsService;


    private int _totalReports;
    private int _approvedReports;
    private int _pendingReports;
    private int _todayReports;
    private int _totalFindings;
    private int _totalImages;


    public ICommand RefreshCommand { get; }


    public ReportDashboardViewModel()
    {
        _statisticsService =
            new ReportStatisticsService();


        RefreshCommand =
            new RelayCommand(
                _ => Refresh());


        Refresh();
    }


    public int TotalReports
    {
        get => _totalReports;

        private set
        {
            _totalReports = value;
            OnPropertyChanged();
        }
    }


    public int ApprovedReports
    {
        get => _approvedReports;

        private set
        {
            _approvedReports = value;
            OnPropertyChanged();
        }
    }


    public int PendingReports
    {
        get => _pendingReports;

        private set
        {
            _pendingReports = value;
            OnPropertyChanged();
        }
    }


    public int TodayReports
    {
        get => _todayReports;

        private set
        {
            _todayReports = value;
            OnPropertyChanged();
        }
    }


    public int TotalFindings
    {
        get => _totalFindings;

        private set
        {
            _totalFindings = value;
            OnPropertyChanged();
        }
    }


    public int TotalImages
    {
        get => _totalImages;

        private set
        {
            _totalImages = value;
            OnPropertyChanged();
        }
    }


    public void Refresh()
    {
        TotalReports =
            _statisticsService.GetTotalReports();


        ApprovedReports =
            _statisticsService.GetApprovedReports();


        PendingReports =
            _statisticsService.GetPendingReports();


        TodayReports =
            _statisticsService.GetTodayReports();


        TotalFindings =
            _statisticsService.GetTotalFindings();


        TotalImages =
            _statisticsService.GetTotalImages();
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