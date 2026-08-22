using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Commands;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ReportEditorViewModel : INotifyPropertyChanged
{
    private readonly ReportGeneratorService _reportGeneratorService;
    private readonly ReportValidationService _reportValidationService;
    private readonly ImageService _imageService;

    private ReportDataModel _report;
    private string _statusMessage = string.Empty;
    private ImageRecordModel? _selectedInspectionImage;
    private ReportImageModel? _selectedReportImage;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ReportEditorViewModel()
    {
        _reportGeneratorService =
            new ReportGeneratorService();

        _reportValidationService =
            new ReportValidationService();

        _imageService =
            new ImageService();

        var currentJob =
            CurrentJobService.Instance.CurrentJob;

        _report =
            new ReportDataModel
            {
                ReportNumber =
                    $"PSNDT-RPT-{DateTime.Now:yyyyMMdd-HHmmss}",

                JobId =
                    currentJob?.Id ?? Guid.Empty,

                InspectionDate =
                    DateTime.Now
            };

        Findings =
            new ObservableCollection<ReportFindingModel>();

        Images =
            new ObservableCollection<ReportImageModel>();

        ReviewedImages =
            new ObservableCollection<ImageRecordModel>();

        GeneratePreviewCommand =
            new RelayCommand(
                ExecuteGeneratePreview);

        LoadReviewedImagesCommand =
            new RelayCommand(
                ExecuteLoadReviewedImages);

        AddSelectedImageCommand =
            new RelayCommand(
                ExecuteAddSelectedImage);

        RemoveSelectedImageCommand =
            new RelayCommand(
                ExecuteRemoveSelectedImage);

        LoadReviewedImages();
        LoadDefectFindings();
    }

    public RelayCommand GeneratePreviewCommand
    {
        get;
    }

    public RelayCommand LoadReviewedImagesCommand
    {
        get;
    }

    public RelayCommand AddSelectedImageCommand
    {
        get;
    }

    public RelayCommand RemoveSelectedImageCommand
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

    public ObservableCollection<ImageRecordModel> ReviewedImages
    {
        get;
    }

    public ImageRecordModel? SelectedInspectionImage
    {
        get => _selectedInspectionImage;

        set
        {
            if (_selectedInspectionImage == value)
                return;

            _selectedInspectionImage = value;

            OnPropertyChanged();
        }
    }

    public ReportImageModel? SelectedReportImage
    {
        get => _selectedReportImage;

        set
        {
            if (_selectedReportImage == value)
                return;

            _selectedReportImage = value;

            OnPropertyChanged();
        }
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

    // ============================================================
    // FINDINGS
    // ============================================================

    public void AddFinding(
        ReportFindingModel finding)
    {
        if (finding == null)
        {
            return;
        }

        if (Findings.Any(
                x => x.Id == finding.Id))
        {
            return;
        }

        Findings.Add(finding);

        if (!Report.Findings.Any(
                x => x.Id == finding.Id))
        {
            Report.Findings.Add(finding);
        }

        RenumberFindings();

        StatusMessage =
            "Finding added successfully.";
    }

    public void LoadDefectFindings()
    {
        Findings.Clear();
        Report.Findings.Clear();

        var currentJob =
            CurrentJobService.Instance.CurrentJob;

        if (currentJob == null)
        {
            Report.JobId =
                Guid.Empty;

            return;
        }

        Report.JobId =
            currentJob.Id;

        IEnumerable<DefectModel> defects;

        try
        {
            defects =
                DefectService.Instance.GetByJob(
                    currentJob.Id);
        }
        catch (Exception ex)
        {
            StatusMessage =
                $"Unable to load defects: {ex.Message}";

            return;
        }

        foreach (DefectModel defect in defects
                     .OrderBy(x => x.ShotNumber)
                     .ThenBy(x => x.PipePosition)
                     .ThenBy(x => x.CreatedOn))
        {
            ReportFindingModel finding =
                CreateFindingFromDefect(
                    defect);

            Findings.Add(finding);
            Report.Findings.Add(finding);
        }

        RenumberFindings();

        if (Findings.Count > 0)
        {
            StatusMessage =
                $"{Findings.Count} defect finding(s) loaded automatically.";
        }
    }

    private static ReportFindingModel CreateFindingFromDefect(
        DefectModel defect)
    {
        string type =
            string.IsNullOrWhiteSpace(
                defect.DefectType)
                ? "UNCLASSIFIED"
                : defect.DefectType.Trim();

        string severity =
            string.IsNullOrWhiteSpace(
                defect.Severity)
                ? "UNCLASSIFIED"
                : defect.Severity.Trim();

        string status =
            string.IsNullOrWhiteSpace(
                defect.Status)
                ? "OPEN"
                : defect.Status.Trim();

        string location =
            $"Shot {defect.ShotNumber} / " +
            $"Pipe Position {defect.PipePosition:0.0} mm";

        string description =
            $"Type: {type}; " +
            $"Length: {defect.LengthMm:0.0} mm; " +
            $"Width: {defect.WidthMm:0.0} mm";

        if (!string.IsNullOrWhiteSpace(
                defect.Description))
        {
            description +=
                $"; Remarks: {defect.Description.Trim()}";
        }

        string evaluation =
            $"Status: {status}; " +
            $"Severity: {severity}";

        if (defect.ThicknessChecked)
        {
            evaluation +=
                $"; Thickness: " +
                $"{defect.ActualThicknessMm:0.00} mm";

            if (defect.MinimumThicknessMm > 0)
            {
                evaluation +=
                    $" / Minimum " +
                    $"{defect.MinimumThicknessMm:0.00} mm";
            }

            if (!string.IsNullOrWhiteSpace(
                    defect.ThicknessStatus))
            {
                evaluation +=
                    $" / {defect.ThicknessStatus}";
            }
        }

        if (!string.IsNullOrWhiteSpace(
                defect.ThicknessRemark))
        {
            evaluation +=
                $"; Thickness Remark: " +
                defect.ThicknessRemark.Trim();
        }

        bool accepted =
            string.Equals(
                status,
                "ACCEPT",
                StringComparison.OrdinalIgnoreCase)
            ||
            string.Equals(
                status,
                "ACCEPTED",
                StringComparison.OrdinalIgnoreCase);

        return new ReportFindingModel
        {
            Id =
                defect.Id,

            FindingNumber =
                0,

            Location =
                location,

            FindingType =
                type,

            Description =
                description,

            Severity =
                severity,

            Evaluation =
                evaluation,

            IsAccepted =
                accepted,

            InspectorRemark =
                defect.Description ?? string.Empty,

            CreatedOn =
                defect.CreatedOn
        };
    }

    private void RenumberFindings()
    {
        int sequence =
            1;

        foreach (ReportFindingModel finding
                 in Findings)
        {
            finding.FindingNumber =
                sequence++;
        }
    }

    // ============================================================
    // IMAGES
    // ============================================================

    public void AddImage(
        ReportImageModel image)
    {
        if (image == null)
        {
            return;
        }

        Images.Add(image);

        Report.Images.Add(image);

        StatusMessage =
            "Image added successfully.";
    }

    public void LoadReviewedImages()
    {
        ReviewedImages.Clear();

        var currentJob =
            CurrentJobService.Instance.CurrentJob;

        if (currentJob == null)
        {
            Report.JobId =
                Guid.Empty;

            StatusMessage =
                "No current job is open.";

            return;
        }

        Report.JobId =
            currentJob.Id;

        List<ImageRecordModel> images;

        try
        {
            images =
                _imageService.GetByJob(
                    currentJob.Id);
        }
        catch (Exception ex)
        {
            StatusMessage =
                $"Unable to load inspection images: {ex.Message}";

            return;
        }

        foreach (var image in images
                     .Where(IsReviewedImage)
                     .OrderBy(x => x.FrameNumber))
        {
            ReviewedImages.Add(image);
        }

        if (ReviewedImages.Count == 0)
        {
            StatusMessage =
                "No reviewed images available for this job.";

            return;
        }

        StatusMessage =
            $"{ReviewedImages.Count} reviewed image(s) loaded automatically.";
    }

    // ============================================================
    // REPORT PREVIEW
    // ============================================================

    public string GeneratePreview()
    {
        LoadDefectFindings();

        if (!_reportValidationService.Validate(
                Report))
        {
            StatusMessage =
                "Report validation failed.";

            return string.Empty;
        }

        Report.GeneratedDate =
            DateTime.Now;

        StatusMessage =
            "Report generated.";

        return _reportGeneratorService
            .GenerateReportSummary(
                Report);
    }

    // ============================================================
    // COMMANDS
    // ============================================================

    private void ExecuteGeneratePreview(
        object? parameter)
    {
        GeneratePreview();
    }

    private void ExecuteLoadReviewedImages(
        object? parameter)
    {
        LoadReviewedImages();
        LoadDefectFindings();
    }

    private void ExecuteAddSelectedImage(
        object? parameter)
    {
        if (SelectedInspectionImage == null)
        {
            StatusMessage =
                "Please select an inspection image.";

            return;
        }

        var source =
            SelectedInspectionImage;

        if (Images.Any(
                x => x.FilePath == source.FilePath))
        {
            StatusMessage =
                "Image is already added to the report.";

            return;
        }

        var reportImage =
            new ReportImageModel
            {
                Id =
                    Guid.NewGuid(),

                ReportId =
                    Report.Id,

                ImageName =
                    source.FileName,

                SequenceNumber =
                    Images.Count + 1,

                FilePath =
                    source.FilePath,

                FileName =
                    source.FileName,

                ImageType =
                    "RT IMAGE",

                AddedOn =
                    DateTime.Now,

                AddedBy =
                    Environment.UserName,

                CapturedOn =
                    source.CapturedOn,

                CapturedBy =
                    source.Operator,

                Description =
                    $"Frame {source.FrameNumber}",

                Remarks =
                    source.Remarks
            };

        AddImage(
            reportImage);

        SelectedReportImage =
            reportImage;

        StatusMessage =
            $"Frame {source.FrameNumber} added to report.";
    }

    private void ExecuteRemoveSelectedImage(
        object? parameter)
    {
        if (SelectedReportImage == null)
        {
            StatusMessage =
                "Please select a report image to remove.";

            return;
        }

        var image =
            SelectedReportImage;

        Images.Remove(
            image);

        Report.Images.Remove(
            image);

        RenumberReportImages();

        SelectedReportImage =
            null;

        StatusMessage =
            $"{image.FileName} removed from report.";
    }

    private void RenumberReportImages()
    {
        int sequence =
            1;

        foreach (var image in Images)
        {
            image.SequenceNumber =
                sequence++;
        }
    }

    private static bool IsReviewedImage(
        ImageRecordModel image)
    {
        return
            string.Equals(
                image.ReviewStatus,
                "ACCEPT",
                StringComparison.OrdinalIgnoreCase)
            ||
            string.Equals(
                image.ReviewStatus,
                "ACCEPTED",
                StringComparison.OrdinalIgnoreCase)
            ||
            string.Equals(
                image.ReviewStatus,
                "REJECT",
                StringComparison.OrdinalIgnoreCase)
            ||
            string.Equals(
                image.ReviewStatus,
                "REJECTED",
                StringComparison.OrdinalIgnoreCase)
            ||
            string.Equals(
                image.ReviewStatus,
                "HOLD",
                StringComparison.OrdinalIgnoreCase)
            ||
            string.Equals(
                image.ReviewStatus,
                "PENDING",
                StringComparison.OrdinalIgnoreCase);
    }

    // ============================================================
    // PROPERTY CHANGED
    // ============================================================

    private void OnPropertyChanged(
        [CallerMemberName]
        string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(
                propertyName));
    }
}