using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
    private readonly ReportRepository _reportRepository;
    private readonly ImageService _imageService;
    private readonly CurrentJobService _currentJobService;

private ReportDataModel _currentReport;

    private string _generatedReport = string.Empty;
    private string _exportedFilePath = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ReportViewModel()
    {
        _reportGeneratorService = new ReportGeneratorService();
        _pdfExportService = new PdfExportService();
        _reportHistoryService = new ReportHistoryService();
        _reportRepository = new ReportRepository();
        _imageService = new ImageService();
        _currentJobService = CurrentJobService.Instance;

        _currentReport = new ReportDataModel
        {
            Id = Guid.NewGuid(),
            ReportNumber = $"RPT-{DateTime.Now:yyyyMMdd-HHmmss}",
            InspectionDate = DateTime.Now,
            GeneratedDate = DateTime.Now
        };

        Findings = new ObservableCollection<ReportFindingModel>();
        Images = new ObservableCollection<ReportImageModel>();

        GenerateReportCommand = new RelayCommand(_ => GenerateReport());
        ExportPdfCommand = new RelayCommand(_ => ExportPdf());

        _currentJobService.CurrentJobChanged += OnCurrentJobChanged;
        _currentJobService.CurrentWorkOrderChanged += OnCurrentWorkOrderChanged;

        ApplyCurrentJobData();
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

    public ICommand GenerateReportCommand { get; }

    public ICommand ExportPdfCommand { get; }

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
        ApplyCurrentJobData();

        LoadCurrentJobImages();

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

        CurrentReport.GeneratedDate = DateTime.Now;

        _reportRepository.Save(CurrentReport);

        GeneratedReport =
            _reportGeneratorService.GenerateReportSummary(CurrentReport);

        _reportHistoryService.Add(
            new ReportHistoryModel
            {
                Id = Guid.NewGuid(),
                ReportId = CurrentReport.Id,
                ReportNumber = CurrentReport.ReportNumber,
                Version = "1.0",
                Action = "Generate",
                Description = "Report generated and stored",
                PerformedBy = "Current User",
                PerformedOn = DateTime.Now
            });
    }

    public void ExportPdf()
    {
        ApplyCurrentJobData();

        LoadCurrentJobImages();

        if (string.IsNullOrWhiteSpace(GeneratedReport))
        {
            GenerateReport();
        }

        ExportedFilePath =
            _pdfExportService.Export(CurrentReport);

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

    private void ApplyCurrentJobData()
    {
        var job = _currentJobService.CurrentJob;
        var workOrder = _currentJobService.CurrentWorkOrder;

        if (job == null && workOrder == null)
            return;

        if (job != null)
        {
            CurrentReport.JobId = job.Id;
            CurrentReport.Customer = job.Customer;
            CurrentReport.Project = job.Project;
            CurrentReport.Component = job.Component;
            CurrentReport.WeldNumber = job.WeldNumber;
            CurrentReport.Operator = job.Operator;
            CurrentReport.Procedure = job.Procedure;
            CurrentReport.Material = job.Material;
            CurrentReport.Remarks = job.Remark;
        }

        if (workOrder != null)
        {
            if (!string.IsNullOrWhiteSpace(workOrder.WorkOrderNumber))
            {
                CurrentReport.JobId = workOrder.Id;

                if (string.IsNullOrWhiteSpace(CurrentReport.ReportNumber) ||
                    CurrentReport.ReportNumber.StartsWith(
                        "RPT-",
                        StringComparison.OrdinalIgnoreCase))
                {
                    CurrentReport.ReportNumber =
                        $"RPT-{workOrder.WorkOrderNumber}-{DateTime.Now:yyyyMMdd-HHmmss}";
                }
            }

            if (!string.IsNullOrWhiteSpace(workOrder.Customer))
                CurrentReport.Customer = workOrder.Customer;

            if (!string.IsNullOrWhiteSpace(workOrder.Project))
                CurrentReport.Project = workOrder.Project;

            if (!string.IsNullOrWhiteSpace(workOrder.Component))
                CurrentReport.Component = workOrder.Component;

            if (!string.IsNullOrWhiteSpace(workOrder.AssignedOperator))
                CurrentReport.Operator = workOrder.AssignedOperator;

            if (!string.IsNullOrWhiteSpace(workOrder.AssignedInspector))
                CurrentReport.Inspector = workOrder.AssignedInspector;

            if (!string.IsNullOrWhiteSpace(workOrder.Procedure))
                CurrentReport.Procedure = workOrder.Procedure;

            if (!string.IsNullOrWhiteSpace(workOrder.Material))
                CurrentReport.Material = workOrder.Material;

            if (!string.IsNullOrWhiteSpace(workOrder.Technique))
                CurrentReport.Technique = workOrder.Technique;

            if (!string.IsNullOrWhiteSpace(workOrder.Remark))
                CurrentReport.Remarks = workOrder.Remark;
        }
    }

    private void LoadCurrentJobImages()
    {
        var jobId = CurrentReport.JobId;

        if (jobId == Guid.Empty)
            return;

        var records = _imageService
            .GetByJob(jobId)
            .OrderBy(x => x.ShotNumber)
            .ThenBy(x => x.CapturedOn)
            .ToList();

        Images.Clear();

        int sequence = 1;

        foreach (var record in records)
        {
            var image = new ReportImageModel
            {
                Id = record.Id,
                ReportId = CurrentReport.Id,
                ImageName = string.IsNullOrWhiteSpace(record.FileName)
                    ? $"Shot {record.ShotNumber}"
                    : record.FileName,
                FileName = record.FileName,
                SequenceNumber = sequence++,
                FilePath = record.FilePath,
                ImageType = "RT",
                FileSizeBytes = GetFileSize(record.FilePath),
                AddedOn = DateTime.Now,
                AddedBy = record.Operator,
                CapturedOn = record.CapturedOn,
                CapturedBy = record.Operator,
                Remarks = record.Remarks,
                Description = BuildImageDescription(record)
            };

            Images.Add(image);
        }

        BuildExposureParameters(records);
        BuildInspectionResult(records);
    }

    private void BuildExposureParameters(
        IReadOnlyCollection<ImageRecordModel> records)
    {
        if (records.Count == 0)
        {
            CurrentReport.ExposureParameters = string.Empty;
            return;
        }

        var builder = new StringBuilder();

        builder.AppendLine($"Images / Shots : {records.Count}");

        var kvValues = records
            .Where(x => x.KV > 0)
            .Select(x => x.KV)
            .Distinct()
            .ToList();

        if (kvValues.Count > 0)
        {
            builder.AppendLine(
                $"kV : {FormatRange(kvValues)}");
        }

        var maValues = records
            .Where(x => x.MA > 0)
            .Select(x => x.MA)
            .Distinct()
            .ToList();

        if (maValues.Count > 0)
        {
            builder.AppendLine(
                $"mA : {FormatRange(maValues)}");
        }

        var exposureValues = records
            .Where(x => x.ExposureTime > 0)
            .Select(x => x.ExposureTime)
            .Distinct()
            .ToList();

        if (exposureValues.Count > 0)
        {
            builder.AppendLine(
                $"Exposure Time : {FormatRange(exposureValues)} s");
        }

        var sfdValues = records
            .Where(x => x.SFD > 0)
            .Select(x => x.SFD)
            .Distinct()
            .ToList();

        if (sfdValues.Count > 0)
        {
            builder.AppendLine(
                $"SFD : {FormatRange(sfdValues)} mm");
        }

        var oddValues = records
            .Where(x => x.ODD > 0)
            .Select(x => x.ODD)
            .Distinct()
            .ToList();

        if (oddValues.Count > 0)
        {
            builder.AppendLine(
                $"ODD : {FormatRange(oddValues)} mm");
        }

        var thicknessValues = records
            .Where(x => x.MaterialThickness > 0)
            .Select(x => x.MaterialThickness)
            .Distinct()
            .ToList();

        if (thicknessValues.Count > 0)
        {
            builder.AppendLine(
                $"Material Thickness : {FormatRange(thicknessValues)} mm");
        }

        var detector = records
            .Select(x => x.DetectorName)
            .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

        if (!string.IsNullOrWhiteSpace(detector))
        {
            builder.AppendLine($"Detector : {detector}");
        }

        var filter = records
            .Select(x => x.Filter)
            .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

        if (!string.IsNullOrWhiteSpace(filter))
        {
            builder.AppendLine($"Filter : {filter}");
        }

        var iqiType = records
            .Select(x => x.IQIType)
            .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

        if (!string.IsNullOrWhiteSpace(iqiType))
        {
            builder.AppendLine($"IQI Type : {iqiType}");
        }

        var iqiSensitivity = records
            .Select(x => x.IQISensitivity)
            .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

        if (!string.IsNullOrWhiteSpace(iqiSensitivity))
        {
            builder.AppendLine(
                $"IQI Sensitivity : {iqiSensitivity}");
        }

        CurrentReport.ExposureParameters =
            builder.ToString().TrimEnd();
    }

    private void BuildInspectionResult(
        IReadOnlyCollection<ImageRecordModel> records)
    {
        if (records.Count == 0)
        {
            CurrentReport.Result = string.Empty;
            return;
        }

        bool hasRejected = records.Any(
            x => string.Equals(
                x.ReviewStatus,
                "REJECTED",
                StringComparison.OrdinalIgnoreCase));

        bool hasPending = records.Any(
            x => string.Equals(
                x.ReviewStatus,
                "PENDING",
                StringComparison.OrdinalIgnoreCase) ||
                string.IsNullOrWhiteSpace(x.ReviewStatus));

        if (hasRejected)
        {
            CurrentReport.Result = "REJECTED";
        }
        else if (hasPending)
        {
            CurrentReport.Result = "PENDING";
        }
        else
        {
            CurrentReport.Result = "ACCEPTED";
        }
    }

    private static string BuildImageDescription(
        ImageRecordModel record)
    {
        var parts = new ObservableCollection<string>();

        if (record.ShotNumber > 0)
        {
            parts.Add($"Shot {record.ShotNumber}");
        }

        if (record.TotalShots > 0)
        {
            parts.Add($"of {record.TotalShots}");
        }

        if (!string.IsNullOrWhiteSpace(record.PipeId))
        {
            parts.Add($"Pipe {record.PipeId}");
        }

        if (!string.IsNullOrWhiteSpace(record.WeldNumber))
        {
            parts.Add($"Weld {record.WeldNumber}");
        }

        if (!string.IsNullOrWhiteSpace(record.JointNumber))
        {
            parts.Add($"Joint {record.JointNumber}");
        }

        if (!string.IsNullOrWhiteSpace(record.ReviewStatus))
        {
            parts.Add(record.ReviewStatus);
        }

        return string.Join(" | ", parts);
    }

    private static string FormatRange(
        IReadOnlyCollection<double> values)
    {
        if (values.Count == 0)
            return string.Empty;

        if (values.Count == 1)
            return values.First().ToString("0.##");

        return $"{values.Min():0.##} - {values.Max():0.##}";
    }

    private static long GetFileSize(string filePath)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(filePath) &&
                System.IO.File.Exists(filePath))
            {
                return new System.IO.FileInfo(filePath).Length;
            }
        }
        catch
        {
        }

        return 0;
    }

    private void OnCurrentJobChanged(
        object? sender,
        JobModel? job)
    {
        ApplyCurrentJobData();
    }

    private void OnCurrentWorkOrderChanged(
        object? sender,
        WorkOrderModel? workOrder)
    {
        ApplyCurrentJobData();
    }

    private void OnPropertyChanged(
        [CallerMemberName]
    string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }


}
