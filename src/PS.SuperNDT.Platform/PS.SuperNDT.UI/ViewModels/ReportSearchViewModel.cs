using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ReportSearchViewModel : INotifyPropertyChanged
{
    private readonly ReportSearchService _searchService;


    public ObservableCollection<ReportDataModel> Reports { get; } =
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


    private string _searchText = string.Empty;


    public string SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText == value)
                return;

            _searchText = value;
            OnPropertyChanged();

            Search();
        }
    }


    public ReportSearchViewModel()
    {
        _searchService =
            new ReportSearchService();

        Search();
    }


    private void Search()
    {
        Reports.Clear();


        foreach (var report in _searchService.Search(SearchText))
        {
            Reports.Add(report);
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