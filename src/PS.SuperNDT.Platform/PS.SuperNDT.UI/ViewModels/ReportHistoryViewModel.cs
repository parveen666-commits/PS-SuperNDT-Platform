using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ReportHistoryViewModel : INotifyPropertyChanged
{
    private readonly ReportHistoryService _reportHistoryService;

    public ObservableCollection<ReportHistoryModel> History { get; } =
        new();

    private ReportHistoryModel? _selectedHistory;

    public ReportHistoryModel? SelectedHistory
    {
        get => _selectedHistory;
        set
        {
            if (_selectedHistory == value)
                return;

            _selectedHistory = value;
            OnPropertyChanged();
        }
    }

    public ReportHistoryViewModel()
    {
        _reportHistoryService =
            new ReportHistoryService();

        LoadHistory();
    }

    public void LoadHistory()
    {
        History.Clear();

        foreach (var item in _reportHistoryService.GetAll())
        {
            History.Add(item);
        }
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