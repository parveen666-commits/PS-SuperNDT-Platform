using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ReportExportViewModel : INotifyPropertyChanged
{
    private readonly ReportExportService _exportService;


    private string _exportPath = string.Empty;


    public ReportDataModel CurrentReport { get; }


    public string ExportPath
    {
        get => _exportPath;
        private set
        {
            if (_exportPath == value)
                return;

            _exportPath = value;
            OnPropertyChanged();
        }
    }


    public ReportExportViewModel()
    {
        _exportService =
            new ReportExportService();

        CurrentReport =
            new ReportDataModel();
    }


    public void Export()
    {
        ExportPath =
            _exportService.ExportReport(
                CurrentReport);
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