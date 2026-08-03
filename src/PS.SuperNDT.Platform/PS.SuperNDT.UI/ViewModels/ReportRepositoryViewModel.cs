using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ReportRepositoryViewModel : INotifyPropertyChanged
{
    private readonly ReportRepository _repository;

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

    public ReportRepositoryViewModel()
    {
        _repository = new ReportRepository();

        Load();
    }

    public void Load()
    {
        Reports.Clear();

        foreach (var report in _repository.GetAll())
        {
            Reports.Add(report);
        }
    }

    public void Save()
    {
        if (SelectedReport == null)
            return;

        _repository.Save(SelectedReport);

        Load();
    }

    public void Delete()
    {
        if (SelectedReport == null)
            return;

        _repository.Delete(SelectedReport.Id);

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