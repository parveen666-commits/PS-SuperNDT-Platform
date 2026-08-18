using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Commands;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ReviewViewModel : INotifyPropertyChanged
{
    private readonly ImageService _imageService = new();

    public ObservableCollection<ImageRecordModel> Images { get; } = new();

    public ObservableCollection<ImageRecordModel> FilteredImages { get; } = new();

    private ImageRecordModel? _selectedImage;

    private string _jobNumberFilter = string.Empty;
    private string _operatorFilter = string.Empty;
    private string _detectorFilter = string.Empty;
    private string _frameFilter = string.Empty;
    private string _statusFilter = "ALL";

    private double _zoomLevel = 1.0;

    private bool _hasPreviousImage;
    private bool _hasNextImage;

    public RelayCommand ClearFilterCommand { get; }

    public RelayCommand ZoomInCommand { get; }

    public RelayCommand ZoomOutCommand { get; }

    public RelayCommand ResetZoomCommand { get; }

    public RelayCommand PreviousImageCommand { get; }

    public RelayCommand NextImageCommand { get; }

    public RelayCommand AcceptCommand { get; }

    public RelayCommand RejectCommand { get; }

    public RelayCommand HoldCommand { get; }

    public string[] ReviewStatuses { get; } =
    {
        "ALL",
        "PENDING",
        "ACCEPT",
        "REJECT",
        "HOLD"
    };

    public double ZoomLevel
    {
        get => _zoomLevel;
        private set
        {
            if (Math.Abs(_zoomLevel - value) < 0.001)
                return;

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
                return;

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
                return;

            _hasNextImage = value;

            OnPropertyChanged();
        }
    }

    public string JobNumberFilter
    {
        get => _jobNumberFilter;
        set
        {
            if (_jobNumberFilter == value)
                return;

            _jobNumberFilter = value;

            OnPropertyChanged();

            ApplyFilter();
        }
    }

    public string OperatorFilter
    {
        get => _operatorFilter;
        set
        {
            if (_operatorFilter == value)
                return;

            _operatorFilter = value;

            OnPropertyChanged();

            ApplyFilter();
        }
    }

    public string DetectorFilter
    {
        get => _detectorFilter;
        set
        {
            if (_detectorFilter == value)
                return;

            _detectorFilter = value;

            OnPropertyChanged();

            ApplyFilter();
        }
    }

    public string FrameFilter
    {
        get => _frameFilter;
        set
        {
            if (_frameFilter == value)
                return;

            _frameFilter = value;

            OnPropertyChanged();

            ApplyFilter();
        }
    }

    public string StatusFilter
    {
        get => _statusFilter;
        set
        {
            if (_statusFilter == value)
                return;

            _statusFilter = value;

            OnPropertyChanged();

            ApplyFilter();
        }
    }

    public ImageRecordModel? SelectedImage
    {
        get => _selectedImage;
        set
        {
            if (ReferenceEquals(_selectedImage, value))
                return;

            _selectedImage = value;

            OnPropertyChanged();

            ResetZoom();

            UpdateNavigationState();

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

    public ReviewViewModel()
    {
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

        AcceptCommand =
            new RelayCommand(
                _ => SetReviewStatus("ACCEPT"));

        RejectCommand =
            new RelayCommand(
                _ => SetReviewStatus("REJECT"));

        HoldCommand =
            new RelayCommand(
                _ => SetReviewStatus("HOLD"));

        LoadImages();

        ImageViewerService.Instance.CurrentImageChanged +=
            ImageViewerService_CurrentImageChanged;

        var currentImage =
            ImageViewerService.Instance.CurrentImage;

        if (currentImage != null)
        {
            _selectedImage = currentImage;

            OnPropertyChanged(
                nameof(SelectedImage));

            UpdateNavigationState();
        }
        else if (FilteredImages.Count > 0)
        {
            SelectedImage =
                FilteredImages[0];
        }
    }

    private void ImageViewerService_CurrentImageChanged(
        object? sender,
        EventArgs e)
    {
        var currentImage =
            ImageViewerService.Instance.CurrentImage;

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

        ResetZoom();

        UpdateNavigationState();
    }

    private void LoadImages()
    {
        Images.Clear();

        var records =
            _imageService.GetAll();

        foreach (var record in records)
        {
            Images.Add(record);
        }

        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var filtered =
            Images
                .Where(image =>
                    string.IsNullOrWhiteSpace(
                        JobNumberFilter)
                    ||
                    image.JobNumber.Contains(
                        JobNumberFilter,
                        StringComparison.OrdinalIgnoreCase))

                .Where(image =>
                    string.IsNullOrWhiteSpace(
                        OperatorFilter)
                    ||
                    image.Operator.Contains(
                        OperatorFilter,
                        StringComparison.OrdinalIgnoreCase))

                .Where(image =>
                    string.IsNullOrWhiteSpace(
                        DetectorFilter)
                    ||
                    image.DetectorName.Contains(
                        DetectorFilter,
                        StringComparison.OrdinalIgnoreCase))

                .Where(image =>
                    string.IsNullOrWhiteSpace(
                        FrameFilter)
                    ||
                    image.FrameNumber
                        .ToString()
                        .Contains(
                            FrameFilter,
                            StringComparison.OrdinalIgnoreCase))

                .Where(image =>
                    StatusFilter == "ALL"
                    ||
                    string.Equals(
                        image.ReviewStatus,
                        StatusFilter,
                        StringComparison.OrdinalIgnoreCase))

                .ToList();

        FilteredImages.Clear();

        foreach (var image in filtered)
        {
            FilteredImages.Add(image);
        }

        if (_selectedImage != null &&
            !FilteredImages.Contains(_selectedImage))
        {
            SelectedImage = null;
        }

        if (_selectedImage == null &&
            FilteredImages.Count > 0)
        {
            SelectedImage =
                FilteredImages[0];
        }

        UpdateNavigationState();
    }

    private void ClearFilters()
    {
        JobNumberFilter = string.Empty;
        OperatorFilter = string.Empty;
        DetectorFilter = string.Empty;
        FrameFilter = string.Empty;
        StatusFilter = "ALL";
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
            return;

        var currentIndex =
            FilteredImages.IndexOf(_selectedImage);

        if (currentIndex <= 0)
            return;

        SelectedImage =
            FilteredImages[currentIndex - 1];
    }

    private void NextImage()
    {
        if (_selectedImage == null)
            return;

        var currentIndex =
            FilteredImages.IndexOf(_selectedImage);

        if (currentIndex < 0)
            return;

        if (currentIndex >= FilteredImages.Count - 1)
            return;

        SelectedImage =
            FilteredImages[currentIndex + 1];
    }

    private void UpdateNavigationState()
    {
        if (_selectedImage == null)
        {
            HasPreviousImage = false;
            HasNextImage = false;
            return;
        }

        var currentIndex =
            FilteredImages.IndexOf(_selectedImage);

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

    private void SetReviewStatus(string status)
    {
        if (_selectedImage == null)
            return;

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

        ApplyFilter();
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
}