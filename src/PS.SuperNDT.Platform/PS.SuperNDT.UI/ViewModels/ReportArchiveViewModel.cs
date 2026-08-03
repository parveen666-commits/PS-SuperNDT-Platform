using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ReportArchiveViewModel : INotifyPropertyChanged
{
    private readonly ReportArchiveService _archiveService;

    public ObservableCollection<ReportDataModel> ArchivedReports { get; } =
        new();

    private ReportDataModel? _selectedReport;

    public ReportDataModel? SelectedReport
    {
        get => _selectedReport;
        set
        {
            if (_selectedReport == value)
                return;

            _selectedReport = value;
            OnPropertyChanged();
        }
    }

    private string _statusMessage = string.Empty;

    public string StatusMessage
    {
        get => _statusMessage;
        private set
        {
            if (_statusMessage == value)
                return;

            _statusMessage = value;
            OnPropertyChanged();
        }
    }

    public ReportArchiveViewModel()
    {
        _archiveService = new ReportArchiveService();

        Load();
    }

    private void Load()
    {
        ArchivedReports.Clear();

        foreach (var report in _archiveService.GetAll())
        {
            ArchivedReports.Add(report);
        }
    }

    public void RemoveSelected()
    {
        if (SelectedReport == null)
        {
            StatusMessage = "Select archived report.";
            return;
        }

        _archiveService.Remove(SelectedReport.Id);

        StatusMessage = "Archive entry removed.";

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