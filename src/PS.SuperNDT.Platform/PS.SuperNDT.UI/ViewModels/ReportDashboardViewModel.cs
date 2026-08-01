using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ReportDashboardViewModel : INotifyPropertyChanged
{
    private readonly ReportStorageService _reportStorageService;
    private readonly ImageService _imageService;

    private int _totalReports;
    private int _approvedReports;
    private int _pendingReports;
    private int _totalImages;

    public int TotalReports
    {
        get => _totalReports;
        set
        {
            _totalReports = value;
            OnPropertyChanged();
        }
    }

    public int ApprovedReports
    {
        get => _approvedReports;
        set
        {
            _approvedReports = value;
            OnPropertyChanged();
        }
    }

    public int PendingReports
    {
        get => _pendingReports;
        set
        {
            _pendingReports = value;
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

    public ReportDashboardViewModel()
    {
        _reportStorageService =
            new ReportStorageService();

        _imageService =
            new ImageService();

        LoadDashboard();
    }

    public void LoadDashboard()
    {
        var reports =
            _reportStorageService.GetAll();

        TotalReports =
            reports.Count;

        ApprovedReports =
            reports.Count(x =>
                x.IsApproved);

        PendingReports =
            reports.Count(x =>
                !x.IsApproved);

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