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

    private ImageRecordModel? _selectedImage;
    private BitmapImage? _displayImage;

    private string _searchText = string.Empty;
    private string _reviewStatusFilter = "ALL";

    private double _zoomLevel = 1.0;

    private bool _hasPreviousImage;
    private bool _hasNextImage;

    private string _reviewMessage = "Ready";

    public ObservableCollection<ImageRecordModel> Images { get; } = new();

    public ObservableCollection<ImageRecordModel> FilteredImages { get; } = new();

    public ObservableCollection<RulerTick> RulerTicks { get; } = new();

    public ObservableCollection<string> StatusFilterItems { get; } =
        new()
        {
            "ALL",
            "PENDING",
            "ACCEPTED",
            "REJECTED"
        };

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
        Images.Count(image =>
            string.Equals(
                image.ReviewStatus,
                "PENDING",
                StringComparison.OrdinalIgnoreCase));

    public int AcceptedImages =>
        Images.Count(image =>
            string.Equals(
                image.ReviewStatus,
                "ACCEPTED",
                StringComparison.OrdinalIgnoreCase));

    public int RejectedImages =>
        Images.Count(image =>
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

        LoadImages();

        ImageViewerService.Instance.CurrentImageChanged +=
            ImageViewerService_CurrentImageChanged;

        var currentImage =
            ImageViewerService.Instance.CurrentImage;

        if (currentImage != null &&
            Images.Contains(currentImage))
        {
            _selectedImage = currentImage;

            OnPropertyChanged(
                nameof(SelectedImage));

            LoadDisplayImage();
            UpdateNavigationState();
            UpdateReviewMessage();
            UpdateRuler();
        }
        else if (FilteredImages.Count > 0)
        {
            SelectedImage =
                FilteredImages[0];
        }
        else
        {
            UpdateRuler();
        }
    }

    private void LoadImages()
    {
        try
        {
            Images.Clear();
            FilteredImages.Clear();

            var currentJob =
                CurrentJobService.Instance.CurrentJob;

            if (currentJob == null)
            {
                SelectedImage = null;

                RulerTicks.Clear();

                OnPropertyChanged(
                    nameof(TotalImages));

                OnPropertyChanged(
                    nameof(PendingImages));

                OnPropertyChanged(
                    nameof(AcceptedImages));

                OnPropertyChanged(
                    nameof(RejectedImages));

                HasPreviousImage = false;
                HasNextImage = false;

                ReviewMessage =
                    "No active job selected.";

                return;
            }

            var records =
                _imageService
                    .GetByJob(currentJob.Id)
                    .OrderBy(
                        image => image.ShotNumber)
                    .ThenBy(
                        image => image.CapturedOn)
                    .ToList();

            foreach (var record in records)
            {
                Images.Add(record);
            }

            OnPropertyChanged(
                nameof(TotalImages));

            OnPropertyChanged(
                nameof(PendingImages));

            OnPropertyChanged(
                nameof(AcceptedImages));

            OnPropertyChanged(
                nameof(RejectedImages));

            ApplyFilter();

            if (Images.Count == 0)
            {
                ReviewMessage =
                    $"No images found for Job {currentJob.JobNumber}.";

                RulerTicks.Clear();
            }
            else
            {
                ReviewMessage =
                    $"Loaded {Images.Count} image(s) for Job {currentJob.JobNumber}.";

                UpdateRuler();
            }
        }
        catch (Exception ex)
        {
            Images.Clear();
            FilteredImages.Clear();
            RulerTicks.Clear();

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

    private void CurrentJobService_CurrentJobChanged(
        object? sender,
        JobModel? job)
    {
        LoadImages();
    }

    private void ApplyFilter()
    {
        string search =
            SearchText.Trim();

        string status =
            ReviewStatusFilter.Trim();

        var filtered =
            Images
                .Where(image =>
                {
                    if (!string.Equals(
                            status,
                            "ALL",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        if (string.Equals(
                                status,
                                "ACCEPTED",
                                StringComparison.OrdinalIgnoreCase))
                        {
                            if (!string.Equals(
                                    image.ReviewStatus,
                                    "ACCEPTED",
                                    StringComparison.OrdinalIgnoreCase))
                            {
                                return false;
                            }
                        }
                        else if (string.Equals(
                                     status,
                                     "REJECTED",
                                     StringComparison.OrdinalIgnoreCase))
                        {
                            if (!string.Equals(
                                    image.ReviewStatus,
                                    "REJECTED",
                                    StringComparison.OrdinalIgnoreCase))
                            {
                                return false;
                            }
                        }
                        else if (string.Equals(
                                     status,
                                     "PENDING",
                                     StringComparison.OrdinalIgnoreCase))
                        {
                            if (!string.Equals(
                                    image.ReviewStatus,
                                    "PENDING",
                                    StringComparison.OrdinalIgnoreCase))
                            {
                                return false;
                            }
                        }
                    }

                    if (string.IsNullOrWhiteSpace(search))
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
                    image => image.ShotNumber)
                .ThenBy(
                    image => image.CapturedOn)
                .ToList();

        FilteredImages.Clear();

        foreach (var image in filtered)
        {
            FilteredImages.Add(image);
        }

        if (_selectedImage != null &&
            !FilteredImages.Contains(
                _selectedImage))
        {
            _selectedImage = null;

            OnPropertyChanged(
                nameof(SelectedImage));

            LoadDisplayImage();

            ImageViewerService.Instance.Clear();

            RulerTicks.Clear();
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
        SearchText = string.Empty;
        ReviewStatusFilter = "ALL";

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
            _selectedImage.ReviewStatus =
                status;

            _selectedImage.ReviewedBy =
                Environment.UserName;

            _selectedImage.ReviewedOn =
                DateTime.Now;

            _imageService.Save(
                _selectedImage);

            OnPropertyChanged(
                nameof(SelectedImage));

            OnPropertyChanged(
                nameof(PendingImages));

            OnPropertyChanged(
                nameof(AcceptedImages));

            OnPropertyChanged(
                nameof(RejectedImages));

            ReviewMessage =
                $"Shot {_selectedImage.ShotNumber} marked {status}";

            ApplyFilter();
        }
        catch (Exception ex)
        {
            ReviewMessage =
                $"Review update failed: {ex.Message}";
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

            return;
        }

        if (!Images.Contains(currentImage))
        {
            return;
        }

        if (ReferenceEquals(
                _selectedImage,
                currentImage))
        {
            return;
        }

        _selectedImage =
            currentImage;

        OnPropertyChanged(
            nameof(SelectedImage));

        LoadDisplayImage();

        ResetZoom();

        UpdateNavigationState();

        UpdateReviewMessage();

        UpdateRuler();
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