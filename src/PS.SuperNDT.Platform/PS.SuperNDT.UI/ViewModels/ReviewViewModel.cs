using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using PS.SuperNDT.UI.Commands;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ReviewViewModel : INotifyPropertyChanged
{
    private readonly ImageService _imageService = new();
    private readonly AuditLogService _auditLogService = new();
    private readonly ImageFolderService _imageFolderService = new();

    private ImageRecordModel? _selectedImage;
    private BitmapImage? _displayImage;

    private string _searchText = string.Empty;
    private string _reviewStatusFilter = "ALL";
    private string _selectedWorkOrder = "ALL WORK ORDERS";

    private double _zoomLevel = 1.0;

    private bool _hasPreviousImage;
    private bool _hasNextImage;

    private string _reviewMessage = "Ready";

    public ObservableCollection<ImageRecordModel> Images { get; } = new();

    public ObservableCollection<ImageRecordModel> FilteredImages { get; } = new();

    public ObservableCollection<RulerTick> RulerTicks { get; } = new();

    public ObservableCollection<string> WorkOrderItems { get; } =
        new();

    public ObservableCollection<string> StatusFilterItems { get; } =
        new()
        {
            "ALL",
            "PENDING",
            "ACCEPTED",
            "REJECTED"
        };

    public ObservableCollection<AuditLogModel> ReviewHistory { get; } =
        new();

    public RelayCommand RefreshCommand { get; }

    public RelayCommand ClearFilterCommand { get; }

    public RelayCommand ZoomInCommand { get; }

    public RelayCommand ZoomOutCommand { get; }

    public RelayCommand ResetZoomCommand { get; }

    public RelayCommand PreviousImageCommand { get; }

    public RelayCommand NextImageCommand { get; }

    public RelayCommand ApproveCommand { get; }

    public RelayCommand RejectCommand { get; }

    public RelayCommand PendingCommand { get; }

    public RelayCommand OpenImageCommand { get; }

    public string SelectedWorkOrder
    {
        get => _selectedWorkOrder;

        set
        {
            value ??= "ALL WORK ORDERS";

            if (_selectedWorkOrder == value)
            {
                return;
            }

            _selectedWorkOrder = value;

            OnPropertyChanged();

            ApplyFilter();
        }
    }

    public string SearchText
    {
        get => _searchText;

        set
        {
            value ??= string.Empty;

            if (_searchText == value)
            {
                return;
            }

            _searchText = value;

            OnPropertyChanged();

            ApplyFilter();
        }
    }

    public string ReviewStatusFilter
    {
        get => _reviewStatusFilter;

        set
        {
            value ??= "ALL";

            if (_reviewStatusFilter == value)
            {
                return;
            }

            _reviewStatusFilter = value;

            OnPropertyChanged();

            ApplyFilter();
        }
    }

    public ImageRecordModel? SelectedImage
    {
        get => _selectedImage;

        set
        {
            if (ReferenceEquals(
                    _selectedImage,
                    value))
            {
                return;
            }

            _selectedImage = value;

            OnPropertyChanged();

            ResetZoom();
            LoadDisplayImage();
            UpdateNavigationState();
            UpdateReviewMessage();
            UpdateRuler();
            LoadReviewHistory();

            if (value != null)
            {
                ImageViewerService.Instance.OpenImage(value);
            }
            else
            {
                ImageViewerService.Instance.Clear();
            }
        }
    }

    public BitmapImage? DisplayImage
    {
        get => _displayImage;

        private set
        {
            if (ReferenceEquals(
                    _displayImage,
                    value))
            {
                return;
            }

            _displayImage = value;

            OnPropertyChanged();
        }
    }

    public double ZoomLevel
    {
        get => _zoomLevel;

        private set
        {
            if (Math.Abs(
                    _zoomLevel - value) < 0.001)
            {
                return;
            }

            _zoomLevel = value;

            OnPropertyChanged();
        }
    }

    public bool HasPreviousImage
    {
        get => _hasPreviousImage;

        private set
        {
            if (_hasPreviousImage == value)
            {
                return;
            }

            _hasPreviousImage = value;

            OnPropertyChanged();
        }
    }

    public bool HasNextImage
    {
        get => _hasNextImage;

        private set
        {
            if (_hasNextImage == value)
            {
                return;
            }

            _hasNextImage = value;

            OnPropertyChanged();
        }
    }

    public string ReviewMessage
    {
        get => _reviewMessage;

        private set
        {
            if (_reviewMessage == value)
            {
                return;
            }

            _reviewMessage = value;

            OnPropertyChanged();
        }
    }

    public int TotalImages =>
        Images.Count;

    public int PendingImages =>
        FilteredImages.Count(image =>
            string.Equals(
                image.ReviewStatus,
                "PENDING",
                StringComparison.OrdinalIgnoreCase));

    public int AcceptedImages =>
        FilteredImages.Count(image =>
            string.Equals(
                image.ReviewStatus,
                "ACCEPTED",
                StringComparison.OrdinalIgnoreCase));

    public int RejectedImages =>
        FilteredImages.Count(image =>
            string.Equals(
                image.ReviewStatus,
                "REJECTED",
                StringComparison.OrdinalIgnoreCase));

    public ReviewViewModel()
    {
        RefreshCommand =
            new RelayCommand(
                _ => LoadImages());

        ClearFilterCommand =
            new RelayCommand(
                _ => ClearFilters());

        ZoomInCommand =
            new RelayCommand(
                _ => ZoomIn());

        ZoomOutCommand =
            new RelayCommand(
                _ => ZoomOut());

        ResetZoomCommand =
            new RelayCommand(
                _ => ResetZoom());

        PreviousImageCommand =
            new RelayCommand(
                _ => PreviousImage());

        NextImageCommand =
            new RelayCommand(
                _ => NextImage());

        ApproveCommand =
            new RelayCommand(
                _ => SetReviewStatus("ACCEPTED"));

        RejectCommand =
            new RelayCommand(
                _ => SetReviewStatus("REJECTED"));

        PendingCommand =
            new RelayCommand(
                _ => SetReviewStatus("PENDING"));

        OpenImageCommand =
            new RelayCommand(
                _ => OpenSelectedImage());

        CurrentJobService.Instance.CurrentJobChanged +=
            CurrentJobService_CurrentJobChanged;

        ImageService.ImageSaved +=
            ImageService_ImageSaved;

        ImageViewerService.Instance.CurrentImageChanged +=
            ImageViewerService_CurrentImageChanged;

        LoadImages();

        var currentImage =
            ImageViewerService.Instance.CurrentImage;

        if (currentImage != null)
        {
            var savedImage =
                Images.FirstOrDefault(
                    image => image.Id == currentImage.Id);

            if (savedImage != null)
            {
                _selectedImage = savedImage;

                OnPropertyChanged(
                    nameof(SelectedImage));

                LoadDisplayImage();
                UpdateNavigationState();
                UpdateReviewMessage();
                UpdateRuler();
                LoadReviewHistory();
            }
        }

        if (_selectedImage == null &&
            FilteredImages.Count > 0)
        {
            SelectedImage =
                FilteredImages[0];
        }
        else if (FilteredImages.Count == 0)
        {
            UpdateRuler();
            ReviewHistory.Clear();
        }
    }

    private void LoadImages()
    {
        try
        {
            var previousSelectedId =
                _selectedImage?.Id;

            Images.Clear();
            FilteredImages.Clear();

            var records =
                _imageService
                    .GetAll()
                    .OrderBy(
                        image => image.CapturedOn)
                    .ThenBy(
                        image => image.JobNumber)
                    .ThenBy(
                        image => image.ShotNumber)
                    .ToList();

            foreach (var record in records)
            {
                Images.Add(record);
            }

            BuildWorkOrderList();

            OnPropertyChanged(
                nameof(TotalImages));

            ApplyFilter();

            if (Images.Count == 0)
            {
                SelectedImage = null;

                RulerTicks.Clear();
                ReviewHistory.Clear();

                HasPreviousImage = false;
                HasNextImage = false;

                ReviewMessage =
                    "No saved images found.";

                return;
            }

            if (previousSelectedId.HasValue)
            {
                var previousImage =
                    FilteredImages.FirstOrDefault(
                        image =>
                            image.Id ==
                            previousSelectedId.Value);

                if (previousImage != null)
                {
                    SelectedImage =
                        previousImage;
                }
            }

            ReviewMessage =
                $"Loaded {Images.Count} saved image(s) from all jobs.";

            UpdateRuler();
            LoadReviewHistory();
        }
        catch (Exception ex)
        {
            Images.Clear();
            FilteredImages.Clear();
            WorkOrderItems.Clear();
            RulerTicks.Clear();
            ReviewHistory.Clear();

            SelectedImage = null;

            HasPreviousImage = false;
            HasNextImage = false;

            ReviewMessage =
                $"Review load failed: {ex.Message}";

            OnPropertyChanged(
                nameof(TotalImages));

            OnPropertyChanged(
                nameof(PendingImages));

            OnPropertyChanged(
                nameof(AcceptedImages));

            OnPropertyChanged(
                nameof(RejectedImages));
        }
    }

    private void BuildWorkOrderList()
    {
        var currentSelection =
            SelectedWorkOrder;

        WorkOrderItems.Clear();

        WorkOrderItems.Add(
            "ALL WORK ORDERS");

        var jobs =
            Images
                .Select(
                    image => image.JobNumber)
                .Where(
                    jobNumber =>
                        !string.IsNullOrWhiteSpace(
                            jobNumber))
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .OrderBy(
                    jobNumber => jobNumber,
                    StringComparer.OrdinalIgnoreCase)
                .ToList();

        foreach (var jobNumber in jobs)
        {
            WorkOrderItems.Add(
                jobNumber);
        }

        if (WorkOrderItems.Any(
                item =>
                    string.Equals(
                        item,
                        currentSelection,
                        StringComparison.OrdinalIgnoreCase)))
        {
            _selectedWorkOrder =
                currentSelection;
        }
        else
        {
            _selectedWorkOrder =
                "ALL WORK ORDERS";
        }

        OnPropertyChanged(
            nameof(SelectedWorkOrder));
    }

    private void ImageService_ImageSaved(
        object? sender,
        ImageRecordModel image)
    {
        var selectedWorkOrderBeforeRefresh =
            SelectedWorkOrder;

        LoadImages();

        if (WorkOrderItems.Any(
                item =>
                    string.Equals(
                        item,
                        image.JobNumber,
                        StringComparison.OrdinalIgnoreCase)))
        {
            if (string.Equals(
                    selectedWorkOrderBeforeRefresh,
                    "ALL WORK ORDERS",
                    StringComparison.OrdinalIgnoreCase))
            {
                SelectedWorkOrder =
                    selectedWorkOrderBeforeRefresh;
            }
            else
            {
                SelectedWorkOrder =
                    image.JobNumber;
            }
        }

        var savedImage =
            FilteredImages.FirstOrDefault(
                x => x.Id == image.Id);

        if (savedImage != null)
        {
            SelectedImage =
                savedImage;
        }

        ReviewMessage =
            $"New Shot {image.ShotNumber} saved and loaded in Review.";
    }

    private void CurrentJobService_CurrentJobChanged(
        object? sender,
        JobModel? job)
    {
        if (job == null)
        {
            return;
        }

        LoadImages();

        if (WorkOrderItems.Any(
                item =>
                    string.Equals(
                        item,
                        job.JobNumber,
                        StringComparison.OrdinalIgnoreCase)))
        {
            SelectedWorkOrder =
                job.JobNumber;
        }
        else
        {
            SelectedWorkOrder =
                "ALL WORK ORDERS";
        }
    }

    private void ApplyFilter()
    {
        string search =
            SearchText.Trim();

        string status =
            ReviewStatusFilter.Trim();

        string workOrder =
            SelectedWorkOrder.Trim();

        var filtered =
            Images
                .Where(
                    image =>
                    {
                        if (!string.Equals(
                                workOrder,
                                "ALL WORK ORDERS",
                                StringComparison.OrdinalIgnoreCase))
                        {
                            if (!string.Equals(
                                    image.JobNumber,
                                    workOrder,
                                    StringComparison.OrdinalIgnoreCase))
                            {
                                return false;
                            }
                        }

                        if (!string.Equals(
                                status,
                                "ALL",
                                StringComparison.OrdinalIgnoreCase))
                        {
                            if (!string.Equals(
                                    image.ReviewStatus,
                                    status,
                                    StringComparison.OrdinalIgnoreCase))
                            {
                                return false;
                            }
                        }

                        if (string.IsNullOrWhiteSpace(
                                search))
                        {
                            return true;
                        }

                        return
                            Contains(
                                image.JobNumber,
                                search)
                            ||
                            Contains(
                                image.FileName,
                                search)
                            ||
                            Contains(
                                image.PipeId,
                                search)
                            ||
                            Contains(
                                image.Operator,
                                search)
                            ||
                            Contains(
                                image.DetectorName,
                                search)
                            ||
                            Contains(
                                image.Remarks,
                                search)
                            ||
                            Contains(
                                image.WeldNumber,
                                search)
                            ||
                            Contains(
                                image.JointNumber,
                                search)
                            ||
                            image.FrameNumber
                                .ToString()
                                .Contains(
                                    search,
                                    StringComparison.OrdinalIgnoreCase)
                            ||
                            image.ShotNumber
                                .ToString()
                                .Contains(
                                    search,
                                    StringComparison.OrdinalIgnoreCase)
                            ||
                            image.ShotPosition
                                .Contains(
                                    search,
                                    StringComparison.OrdinalIgnoreCase);
                    })
                .OrderBy(
                    image => image.CapturedOn)
                .ThenBy(
                    image => image.JobNumber)
                .ThenBy(
                    image => image.ShotNumber)
                .ToList();

        FilteredImages.Clear();

        foreach (var image in filtered)
        {
            FilteredImages.Add(image);
        }

        OnPropertyChanged(
            nameof(PendingImages));

        OnPropertyChanged(
            nameof(AcceptedImages));

        OnPropertyChanged(
            nameof(RejectedImages));

        if (_selectedImage != null &&
            !FilteredImages.Any(
                image =>
                    image.Id ==
                    _selectedImage.Id))
        {
            _selectedImage = null;

            OnPropertyChanged(
                nameof(SelectedImage));

            LoadDisplayImage();

            ImageViewerService.Instance.Clear();

            RulerTicks.Clear();
            ReviewHistory.Clear();
        }

        if (_selectedImage == null &&
            FilteredImages.Count > 0)
        {
            SelectedImage =
                FilteredImages[0];
        }

        UpdateNavigationState();
        UpdateRuler();

        if (FilteredImages.Count == 0)
        {
            if (Images.Count > 0)
            {
                ReviewMessage =
                    "No images match the current filter.";
            }
            else
            {
                ReviewMessage =
                    "No saved images found.";
            }
        }
        else
        {
            ReviewMessage =
                $"{FilteredImages.Count} image(s) shown";
        }
    }

    private static bool Contains(
        string? value,
        string search)
    {
        return !string.IsNullOrWhiteSpace(value) &&
               value.Contains(
                   search,
                   StringComparison.OrdinalIgnoreCase);
    }

    private void ClearFilters()
    {
        _selectedWorkOrder =
            "ALL WORK ORDERS";

        OnPropertyChanged(
            nameof(SelectedWorkOrder));

        SearchText =
            string.Empty;

        ReviewStatusFilter =
            "ALL";

        ApplyFilter();
    }

    private void ZoomIn()
    {
        ZoomLevel =
            Math.Min(
                5.0,
                ZoomLevel + 0.25);
    }

    private void ZoomOut()
    {
        ZoomLevel =
            Math.Max(
                0.25,
                ZoomLevel - 0.25);
    }

    private void ResetZoom()
    {
        ZoomLevel = 1.0;
    }

    private void PreviousImage()
    {
        if (_selectedImage == null)
        {
            return;
        }

        int currentIndex =
            FilteredImages.IndexOf(
                _selectedImage);

        if (currentIndex <= 0)
        {
            return;
        }

        SelectedImage =
            FilteredImages[
                currentIndex - 1];
    }

    private void NextImage()
    {
        if (_selectedImage == null)
        {
            return;
        }

        int currentIndex =
            FilteredImages.IndexOf(
                _selectedImage);

        if (currentIndex < 0 ||
            currentIndex >=
            FilteredImages.Count - 1)
        {
            return;
        }

        SelectedImage =
            FilteredImages[
                currentIndex + 1];
    }

    private void UpdateNavigationState()
    {
        if (_selectedImage == null)
        {
            HasPreviousImage = false;
            HasNextImage = false;

            return;
        }

        int currentIndex =
            FilteredImages.IndexOf(
                _selectedImage);

        if (currentIndex < 0)
        {
            HasPreviousImage = false;
            HasNextImage = false;

            return;
        }

        HasPreviousImage =
            currentIndex > 0;

        HasNextImage =
            currentIndex <
            FilteredImages.Count - 1;
    }

    private void SetReviewStatus(
        string status)
    {
        if (_selectedImage == null)
        {
            ReviewMessage =
                "Select an image first.";

            return;
        }

        try
        {
            string previousStatus =
                _selectedImage.ReviewStatus;

            string reviewer =
                Environment.UserName;

            DateTime reviewTime =
                DateTime.Now;

            /*
             * Folder workflow:
             *
             * ACCEPTED -> ACCEPT
             * REJECTED -> REJECT
             * PENDING  -> REPAIR
             */
            string folderStatus =
                status switch
                {
                    "ACCEPTED" => "ACCEPT",
                    "REJECTED" => "REJECT",
                    "PENDING" => "REPAIR",
                    _ => string.Empty
                };

            string oldFilePath =
                _selectedImage.FilePath;

            /*
             * Move the physical image first.
             * If there is no physical file path, the review
             * status can still be saved normally.
             */
            if (!string.IsNullOrWhiteSpace(
                    folderStatus) &&
                !string.IsNullOrWhiteSpace(
                    oldFilePath) &&
                File.Exists(oldFilePath))
            {
                _imageFolderService.MoveImageToStatus(
                    _selectedImage,
                    folderStatus);

                string newFilePath =
                    _imageFolderService.GetImagePath(
                        _selectedImage,
                        folderStatus);

                if (!string.IsNullOrWhiteSpace(
                        newFilePath) &&
                    File.Exists(newFilePath))
                {
                    _selectedImage.FilePath =
                        newFilePath;
                }
            }

            _selectedImage.ReviewStatus =
                status;

            _selectedImage.ReviewedBy =
                reviewer;

            _selectedImage.ReviewedOn =
                reviewTime;

            _imageService.Save(
                _selectedImage);

            try
            {
                _auditLogService.Add(
                    reviewer,
                    $"REVIEW_{status}",
                    "Review",
                    $"Job/Work Order: {_selectedImage.JobNumber} | " +
                    $"Shot: {_selectedImage.ShotNumber}/{_selectedImage.TotalShots} | " +
                    $"Pipe: {_selectedImage.PipeId} | " +
                    $"Position: {_selectedImage.ShotPosition} | " +
                    $"Previous Status: {previousStatus} | " +
                    $"New Status: {status} | " +
                    $"Folder: {folderStatus} | " +
                    $"Reviewer: {reviewer} | " +
                    $"Reviewed On: {reviewTime:yyyy-MM-dd HH:mm:ss}");
            }
            catch
            {
                // Audit failure must not prevent review status from being saved.
            }

            OnPropertyChanged(
                nameof(SelectedImage));

            OnPropertyChanged(
                nameof(PendingImages));

            OnPropertyChanged(
                nameof(AcceptedImages));

            OnPropertyChanged(
                nameof(RejectedImages));

            LoadDisplayImage();
            LoadReviewHistory();

            if (string.IsNullOrWhiteSpace(
                    folderStatus))
            {
                ReviewMessage =
                    $"Shot {_selectedImage.ShotNumber} marked {status}.";
            }
            else
            {
                ReviewMessage =
                    $"Shot {_selectedImage.ShotNumber} marked {status} and moved to {folderStatus}.";
            }

            ApplyFilter();
        }
        catch (Exception ex)
        {
            ReviewMessage =
                $"Review update failed: {ex.Message}";
        }
    }

    private void LoadReviewHistory()
    {
        ReviewHistory.Clear();

        if (_selectedImage == null)
        {
            return;
        }

        try
        {
            string jobNumber =
                _selectedImage.JobNumber ?? string.Empty;

            string shotNumber =
                _selectedImage.ShotNumber.ToString();

            var history =
                _auditLogService
                    .GetAll()
                    .Where(
                        log =>
                            string.Equals(
                                log.Module,
                                "Review",
                                StringComparison.OrdinalIgnoreCase)
                            &&
                            log.Description.Contains(
                                $"Job/Work Order: {jobNumber}",
                                StringComparison.OrdinalIgnoreCase)
                            &&
                            log.Description.Contains(
                                $"Shot: {shotNumber}/",
                                StringComparison.OrdinalIgnoreCase))
                    .OrderByDescending(
                        log => log.Timestamp)
                    .ToList();

            foreach (var log in history)
            {
                ReviewHistory.Add(log);
            }
        }
        catch
        {
            ReviewHistory.Clear();
        }
    }

    private void OpenSelectedImage()
    {
        if (_selectedImage == null)
        {
            ReviewMessage =
                "Select an image first.";

            return;
        }

        try
        {
            ImageViewerService.Instance.OpenImage(
                _selectedImage);

            ReviewMessage =
                $"Opened Shot {_selectedImage.ShotNumber}";
        }
        catch (Exception ex)
        {
            ReviewMessage =
                $"Unable to open image: {ex.Message}";
        }
    }

    private void ImageViewerService_CurrentImageChanged(
        object? sender,
        EventArgs e)
    {
        var currentImage =
            ImageViewerService.Instance.CurrentImage;

        if (currentImage == null)
        {
            _selectedImage = null;

            OnPropertyChanged(
                nameof(SelectedImage));

            LoadDisplayImage();

            UpdateNavigationState();

            RulerTicks.Clear();
            ReviewHistory.Clear();

            return;
        }

        var matchingImage =
            Images.FirstOrDefault(
                image =>
                    image.Id ==
                    currentImage.Id);

        if (matchingImage == null)
        {
            return;
        }

        if (ReferenceEquals(
                _selectedImage,
                matchingImage))
        {
            return;
        }

        _selectedImage =
            matchingImage;

        OnPropertyChanged(
            nameof(SelectedImage));

        LoadDisplayImage();

        ResetZoom();

        UpdateNavigationState();

        UpdateReviewMessage();

        UpdateRuler();

        LoadReviewHistory();
    }

    private void LoadDisplayImage()
    {
        DisplayImage = null;

        if (_selectedImage == null)
        {
            return;
        }

        string filePath =
            _selectedImage.FilePath;

        if (string.IsNullOrWhiteSpace(
                filePath))
        {
            return;
        }

        if (!File.Exists(
                filePath))
        {
            return;
        }

        try
        {
            var bitmap =
                new BitmapImage();

            using var stream =
                new FileStream(
                    filePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.ReadWrite);

            bitmap.BeginInit();

            bitmap.CacheOption =
                BitmapCacheOption.OnLoad;

            bitmap.StreamSource =
                stream;

            bitmap.EndInit();

            bitmap.Freeze();

            DisplayImage =
                bitmap;
        }
        catch
        {
            DisplayImage = null;
        }
    }

    private void UpdateReviewMessage()
    {
        if (_selectedImage == null)
        {
            return;
        }

        ReviewMessage =
            $"Job {_selectedImage.JobNumber}  |  " +
            $"Pipe {_selectedImage.PipeId}  |  " +
            $"Shot {_selectedImage.ShotNumber}/" +
            $"{_selectedImage.TotalShots}  |  " +
            $"{_selectedImage.ShotStartPosition:0}-" +
            $"{_selectedImage.ShotEndPosition:0} mm";
    }

    private void UpdateRuler()
    {
        RulerTicks.Clear();

        if (_selectedImage == null)
        {
            return;
        }

        double start =
            _selectedImage.ShotStartPosition;

        double end =
            _selectedImage.ShotEndPosition;

        if (end <= start)
        {
            return;
        }

        double current =
            start;

        const double minorStep = 10.0;
        const double majorStep = 50.0;

        while (current < end)
        {
            double relative =
                current - start;

            bool isMajor =
                Math.Abs(
                    relative % majorStep) < 0.001;

            RulerTicks.Add(
                new RulerTick
                {
                    Position = current,
                    RelativePosition = relative,
                    IsMajor = isMajor,
                    Label = isMajor
                        ? $"{current:0}"
                        : string.Empty
                });

            current += minorStep;
        }

        RulerTicks.Add(
            new RulerTick
            {
                Position = end,
                RelativePosition = end - start,
                IsMajor = true,
                Label = $"{end:0}"
            });
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(
        [CallerMemberName]
        string propertyName = "")
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(
                propertyName));
    }

    public sealed class RulerTick
    {
        public double Position { get; init; }

        public double RelativePosition { get; init; }

        public bool IsMajor { get; init; }

        public string Label { get; init; } = string.Empty;
    }
}