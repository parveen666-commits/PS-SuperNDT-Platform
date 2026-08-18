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

    private double _zoomLevel = 1.0;

    public RelayCommand ClearFilterCommand { get; }

    public RelayCommand ZoomInCommand { get; }

    public RelayCommand ZoomOutCommand { get; }

    public RelayCommand ResetZoomCommand { get; }

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

    public ImageRecordModel? SelectedImage
    {
        get => _selectedImage;
        set
        {
            if (ReferenceEquals(_selectedImage, value))
                return;

            _selectedImage = value;

            OnPropertyChanged();

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
    }

    private void ClearFilters()
    {
        JobNumberFilter = string.Empty;
        OperatorFilter = string.Empty;
        DetectorFilter = string.Empty;
        FrameFilter = string.Empty;
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