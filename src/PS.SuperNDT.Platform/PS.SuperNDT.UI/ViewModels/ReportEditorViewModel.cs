using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Commands;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ReportEditorViewModel : INotifyPropertyChanged
{
    private readonly ReportGeneratorService _reportGeneratorService;
    private readonly ReportValidationService _reportValidationService;

    private ReportDataModel _report;
    private string _statusMessage = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ReportEditorViewModel()
    {
        _reportGeneratorService = new ReportGeneratorService();
        _reportValidationService = new ReportValidationService();

        _report = new ReportDataModel
        {
            ReportNumber =
                $"PSNDT-RPT-{DateTime.Now:yyyyMMdd-HHmmss}",

            InspectionDate = DateTime.Now
        };

        Findings = new ObservableCollection<ReportFindingModel>();

        Images = new ObservableCollection<ReportImageModel>();

        GeneratePreviewCommand = new RelayCommand(
            ExecuteGeneratePreview);
    }

    public RelayCommand GeneratePreviewCommand
    {
        get;
    }

    public ReportDataModel Report
    {
        get => _report;
        set
        {
            _report = value;
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

    public string StatusMessage
    {
        get => _statusMessage;
        private set
        {
            _statusMessage = value;
            OnPropertyChanged();
        }
    }

    public void AddFinding(
        ReportFindingModel finding)
    {
        Findings.Add(finding);

        Report.Findings.Add(finding);

        StatusMessage =
            "Finding added successfully.";
    }

    public void AddImage(
        ReportImageModel image)
    {
        Images.Add(image);

        Report.Images.Add(image);

        StatusMessage =
            "Image added successfully.";
    }

    public string GeneratePreview()
    {
        if (!_reportValidationService.Validate(Report))
        {
            StatusMessage =
                "Report validation failed.";

            return string.Empty;
        }

        StatusMessage =
            "Report generated.";

        return _reportGeneratorService
            .GenerateReportSummary(Report);
    }

    private void ExecuteGeneratePreview(
        object? parameter)
    {
        GeneratePreview();
    }

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}