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

        _report =
            new ReportDataModel
            {
                ReportNumber =
                    $"PSNDT-RPT-{DateTime.Now:yyyyMMdd-HHmmss}",

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

    public void LoadReviewedImages()
    {
        ReviewedImages.Clear();

        var currentJob =
            CurrentJobService.Instance.CurrentJob;

        if (currentJob == null)
        {
            StatusMessage =
                "No current job is open.";

            return;
        }

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

    private void ExecuteLoadReviewedImages(
        object? parameter)
    {
        LoadReviewedImages();
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

        AddImage(reportImage);

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

        Images.Remove(image);

        Report.Images.Remove(image);

        RenumberReportImages();

        SelectedReportImage =
            null;

        StatusMessage =
            $"{image.FileName} removed from report.";
    }

    private void RenumberReportImages()
    {
        var sequence =
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
                "REJECT",
                StringComparison.OrdinalIgnoreCase)
            ||
            string.Equals(
                image.ReviewStatus,
                "HOLD",
                StringComparison.OrdinalIgnoreCase);
    }

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}