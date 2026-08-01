using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PS.SuperNDT.UI.Commands;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ReportViewModel : INotifyPropertyChanged
{
    private readonly ReportGeneratorService _reportGeneratorService;
    private readonly PdfExportService _pdfExportService;
    private readonly ReportHistoryService _reportHistoryService;

    private ReportDataModel _currentReport;
    private string _generatedReport = string.Empty;
    private string _exportedFilePath = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ReportViewModel()
    {
        _reportGeneratorService =
            new ReportGeneratorService();

        _pdfExportService =
            new PdfExportService();

        _reportHistoryService =
            new ReportHistoryService();

        _currentReport = new ReportDataModel
        {
            ReportNumber =
                $"RPT-{DateTime.Now:yyyyMMdd-HHmmss}",

            InspectionDate =
                DateTime.Now
        };

        Findings =
            new ObservableCollection<ReportFindingModel>();

        Images =
            new ObservableCollection<ReportImageModel>();

        GenerateReportCommand =
            new RelayCommand(_ => GenerateReport());

        ExportPdfCommand =
            new RelayCommand(_ => ExportPdf());
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

    public ObservableCollection<ReportFindingModel> Findings
    {
        get;
    }

    public ObservableCollection<ReportImageModel> Images
    {
        get;
    }

    public ICommand GenerateReportCommand
    {
        get;
    }

    public ICommand ExportPdfCommand
    {
        get;
    }

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
            _reportGeneratorService
                .GenerateReportSummary(CurrentReport);

        _reportHistoryService.Add(
            new ReportHistoryModel
            {
                Id = Guid.NewGuid(),
                ReportId = CurrentReport.Id,
                ReportNumber = CurrentReport.ReportNumber,
                Version = "1.0",
                Action = "Generate",
                Description = "Report generated",
                PerformedBy = "Current User",
                PerformedOn = DateTime.Now
            });
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

        _reportHistoryService.Add(
            new ReportHistoryModel
            {
                Id = Guid.NewGuid(),
                ReportId = CurrentReport.Id,
                ReportNumber = CurrentReport.ReportNumber,
                Version = "1.0",
                Action = "Export PDF",
                Description = ExportedFilePath,
                PerformedBy = "Current User",
                PerformedOn = DateTime.Now
            });
    }

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}