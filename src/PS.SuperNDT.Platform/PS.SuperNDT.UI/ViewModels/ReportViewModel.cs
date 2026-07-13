using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ReportViewModel : INotifyPropertyChanged
{
    private readonly ReportGeneratorService _reportGeneratorService;
    private readonly PdfExportService _pdfExportService;

    private ReportDataModel _currentReport;
    private string _generatedReport = string.Empty;
    private string _exportedFilePath = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ReportViewModel()
    {
        _reportGeneratorService = new ReportGeneratorService();
        _pdfExportService = new PdfExportService();

        _currentReport = new ReportDataModel
        {
            ReportNumber = $"RPT-{DateTime.Now:yyyyMMdd-HHmmss}",
            InspectionDate = DateTime.Now
        };

        Findings = new ObservableCollection<ReportFindingModel>();
        Images = new ObservableCollection<ReportImageModel>();
    }

    public ReportDataModel CurrentReport
    {
        get => _currentReport;
        set
        {
            _currentReport = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<ReportFindingModel> Findings { get; }

    public ObservableCollection<ReportImageModel> Images { get; }

    public string GeneratedReport
    {
        get => _generatedReport;
        private set
        {
            _generatedReport = value;
            OnPropertyChanged();
        }
    }

    public string ExportedFilePath
    {
        get => _exportedFilePath;
        private set
        {
            _exportedFilePath = value;
            OnPropertyChanged();
        }
    }

    public void GenerateReport()
    {
        CurrentReport.Findings.Clear();

        foreach (var finding in Findings)
        {
            CurrentReport.Findings.Add(finding);
        }

        CurrentReport.Images.Clear();

        foreach (var image in Images)
        {
            CurrentReport.Images.Add(image);
        }

        GeneratedReport =
            _reportGeneratorService.GenerateReportSummary(CurrentReport);
    }

    public void ExportPdf()
    {
        if (string.IsNullOrWhiteSpace(GeneratedReport))
        {
            GenerateReport();
        }

        ExportedFilePath =
            _pdfExportService.Export(
                GeneratedReport,
                CurrentReport.ReportNumber);
    }

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}