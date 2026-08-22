using PS.SuperNDT.UI.Commands;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ReviewViewModel : INotifyPropertyChanged
{
    private readonly ImageService _imageService = new();
    private readonly AuditLogService _auditLogService = new();
    private readonly ImageFolderService _imageFolderService = new();
    private readonly ReviewedImageExportService _reviewedImageExportService = new();

    private ImageRecordModel? _selectedImage;
    private BitmapImage? _displayImage;

    private string _searchText = string.Empty;
    private string _reviewStatusFilter = "ALL";
    private string _selectedWorkOrder = "ALL WORK ORDERS";

    private double _zoomLevel = 1.0;

    private bool _hasPreviousImage;
    private bool _hasNextImage;

    private string _reviewMessage = "Ready";

    // ============================================================
    // IMAGE FILTER
    // ============================================================

    private double _brightness;
    private double _contrast;
    private double _gamma = 1.0;

    private double _snr;
    private string _snrText = "SNR: --";

    public ObservableCollection<ImageRecordModel> Images { get; } =
        new();

    public ObservableCollection<ImageRecordModel> FilteredImages { get; } =
        new();

    public ObservableCollection<RulerTick> RulerTicks { get; } =
        new();

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

    // ============================================================
    // COMMANDS
    // ============================================================

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

    public RelayCommand SaveReviewedPngCommand { get; }

    // Image processing commands
    public RelayCommand ResetImageFilterCommand { get; }

    public RelayCommand ApplyImageFilterCommand { get; }

    // ============================================================
    // BASIC REVIEW FILTERS
    // ============================================================

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

    // ============================================================
    // IMAGE FILTER PROPERTIES
    // ============================================================

    public double Brightness
    {
        get => _brightness;

        set
        {
            double newValue =
                Math.Clamp(
                    value,
                    -100.0,
                    100.0);

            if (Math.Abs(
                    _brightness - newValue) < 0.001)
            {
                return;
            }

            _brightness = newValue;

            OnPropertyChanged();

            ApplyImageFilter();
        }
    }

    public double Contrast
    {
        get => _contrast;

        set
        {
            double newValue =
                Math.Clamp(
                    value,
                    -100.0,
                    100.0);

            if (Math.Abs(
                    _contrast - newValue) < 0.001)
            {
                return;
            }

            _contrast = newValue;

            OnPropertyChanged();

            ApplyImageFilter();
        }
    }

    public double Gamma
    {
        get => _gamma;

        set
        {
            double newValue =
                Math.Clamp(
                    value,
                    0.20,
                    3.00);

            if (Math.Abs(
                    _gamma - newValue) < 0.001)
            {
                return;
            }

            _gamma = newValue;

            OnPropertyChanged();

            ApplyImageFilter();
        }
    }

    public double SNR
    {
        get => _snr;

        private set
        {
            if (Math.Abs(
                    _snr - value) < 0.001)
            {
                return;
            }

            _snr = value;

            OnPropertyChanged();
        }
    }

    public string SNRText
    {
        get => _snrText;

        private set
        {
            if (_snrText == value)
            {
                return;
            }

            _snrText = value;

            OnPropertyChanged();
        }
    }

    // ============================================================
    // SELECTED IMAGE
    // ============================================================

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
            ResetImageFilter();
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

    // ============================================================
    // ZOOM
    // ============================================================

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

    // ============================================================
    // COUNTERS
    // ============================================================

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

    // ============================================================
    // CONSTRUCTOR
    // ============================================================

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

        SaveReviewedPngCommand =
            new RelayCommand(
                _ => SaveReviewedPng());

        ResetImageFilterCommand =
            new RelayCommand(
                _ => ResetImageFilter());

        ApplyImageFilterCommand =
            new RelayCommand(
                _ => ApplyImageFilter());

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
            SNR = 0;
            SNRText = "SNR: --";
        }
    }

    // ============================================================
    // LOAD IMAGES
    // ============================================================

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

                SNR = 0;
                SNRText = "SNR: --";

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

            SNR = 0;
            SNRText = "SNR: --";

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

    // ============================================================
    // WORK ORDER LIST
    // ============================================================

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

    // ============================================================
    // IMAGE SAVED EVENT
    // ============================================================

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

    // ============================================================
    // CURRENT JOB EVENT
    // ============================================================

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

    // ============================================================
    // REVIEW FILTER
    // ============================================================

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

            SNR = 0;
            SNRText = "SNR: --";
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

    // ============================================================
    // ZOOM
    // ============================================================

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

    // ============================================================
    // IMAGE FILTER RESET
    // ============================================================

    private void ResetImageFilter()
    {
        _brightness = 0;
        _contrast = 0;
        _gamma = 1.0;

        OnPropertyChanged(
            nameof(Brightness));

        OnPropertyChanged(
            nameof(Contrast));

        OnPropertyChanged(
            nameof(Gamma));

        ApplyImageFilter();
    }

    // ============================================================
    // IMAGE FILTER
    // ============================================================

    private void ApplyImageFilter()
    {
        if (_selectedImage == null)
        {
            return;
        }

        string filePath =
            _selectedImage.FilePath;

        if (string.IsNullOrWhiteSpace(
                filePath) ||
            !File.Exists(filePath))
        {
            return;
        }

        try
        {
            BitmapImage source =
                LoadBitmap(
                    filePath);

            if (Math.Abs(Brightness) < 0.001 &&
                Math.Abs(Contrast) < 0.001 &&
                Math.Abs(Gamma - 1.0) < 0.001)
            {
                DisplayImage =
                    source;

                CalculateSNR(
                    source);

                return;
            }

            int width =
                source.PixelWidth;

            int height =
                source.PixelHeight;

            if (width <= 0 ||
                height <= 0)
            {
                return;
            }

            WriteableBitmap writable =
                new WriteableBitmap(
                    width,
                    height,
                    source.DpiX,
                    source.DpiY,
                    PixelFormats.Bgra32,
                    null);

            int stride =
                width * 4;

            byte[] pixels =
                new byte[
                    stride *
                    height];

            source.CopyPixels(
                pixels,
                stride,
                0);

            double contrastFactor =
                (100.0 + Contrast) /
                100.0;

            contrastFactor *=
                contrastFactor;

            double brightnessOffset =
                Brightness * 2.55;

            double gammaValue =
                Gamma;

            for (int index = 0;
                 index < pixels.Length;
                 index += 4)
            {
                double blue =
                    pixels[index];

                double green =
                    pixels[index + 1];

                double red =
                    pixels[index + 2];

                blue =
                    ApplyPixelFilter(
                        blue,
                        brightnessOffset,
                        contrastFactor,
                        gammaValue);

                green =
                    ApplyPixelFilter(
                        green,
                        brightnessOffset,
                        contrastFactor,
                        gammaValue);

                red =
                    ApplyPixelFilter(
                        red,
                        brightnessOffset,
                        contrastFactor,
                        gammaValue);

                pixels[index] =
                    (byte)blue;

                pixels[index + 1] =
                    (byte)green;

                pixels[index + 2] =
                    (byte)red;
            }

            writable.WritePixels(
                new Int32Rect(
                    0,
                    0,
                    width,
                    height),
                pixels,
                stride,
                0);

            writable.Freeze();

            DisplayImage =
                ConvertToBitmapImage(
                    writable);

            CalculateSNR(
                DisplayImage);
        }
        catch
        {
            DisplayImage = null;

            SNR = 0;
            SNRText = "SNR: --";
        }
    }

    private static double ApplyPixelFilter(
        double pixel,
        double brightnessOffset,
        double contrastFactor,
        double gamma)
    {
        double normalized =
            pixel / 255.0;

        normalized =
            Math.Clamp(
                normalized +
                brightnessOffset / 255.0,
                0.0,
                1.0);

        normalized =
            ((normalized - 0.5) *
             contrastFactor) +
            0.5;

        normalized =
            Math.Clamp(
                normalized,
                0.0,
                1.0);

        normalized =
            Math.Pow(
                normalized,
                1.0 / gamma);

        return Math.Clamp(
            normalized * 255.0,
            0.0,
            255.0);
    }

    // ============================================================
    // SNR
    // ============================================================

    private void CalculateSNR(
        BitmapImage? bitmap)
    {
        if (bitmap == null ||
            bitmap.PixelWidth <= 0 ||
            bitmap.PixelHeight <= 0)
        {
            SNR = 0;
            SNRText = "SNR: --";

            return;
        }

        try
        {
            int width =
                bitmap.PixelWidth;

            int height =
                bitmap.PixelHeight;

            int stride =
                width * 4;

            byte[] pixels =
                new byte[
                    stride *
                    height];

            bitmap.CopyPixels(
                pixels,
                stride,
                0);

            double sum = 0;
            double sumSquares = 0;

            long pixelCount =
                (long)width *
                height;

            if (pixelCount <= 0)
            {
                SNR = 0;
                SNRText = "SNR: --";

                return;
            }

            for (int index = 0;
                 index < pixels.Length;
                 index += 4)
            {
                double blue =
                    pixels[index];

                double green =
                    pixels[index + 1];

                double red =
                    pixels[index + 2];

                double luminance =
                    (0.114 * blue) +
                    (0.587 * green) +
                    (0.299 * red);

                sum += luminance;

                sumSquares +=
                    luminance *
                    luminance;
            }

            double mean =
                sum / pixelCount;

            double variance =
                (sumSquares /
                 pixelCount) -
                (mean * mean);

            variance =
                Math.Max(
                    0,
                    variance);

            double standardDeviation =
                Math.Sqrt(
                    variance);

            if (standardDeviation < 0.0001 ||
                mean <= 0)
            {
                SNR = 0;

                SNRText =
                    "SNR: HIGH";

                return;
            }

            double ratio =
                mean /
                standardDeviation;

            double snrDb =
                20.0 *
                Math.Log10(
                    Math.Max(
                        ratio,
                        0.000001));

            if (double.IsNaN(snrDb) ||
                double.IsInfinity(snrDb))
            {
                SNR = 0;
                SNRText = "SNR: --";

                return;
            }

            SNR =
                Math.Max(
                    0,
                    snrDb);

            SNRText =
                $"SNR: {SNR:0.0} dB";
        }
        catch
        {
            SNR = 0;
            SNRText = "SNR: --";
        }
    }

    // ============================================================
    // REVIEWED PNG EXPORT
    // ============================================================

    private void SaveReviewedPng()
    {
        if (_selectedImage == null)
        {
            ReviewMessage =
                "Select an image first.";

            return;
        }

        if (DisplayImage == null)
        {
            ReviewMessage =
                "Reviewed image is not available.";

            return;
        }

        try
        {
            var defects =
                DefectService.Instance
                    .GetByImage(
                        _selectedImage.Id)
                    .ToList();

            string destinationPath =
                _reviewedImageExportService
                    .ExportReviewedPng(
                        _selectedImage,
                        DisplayImage,
                        defects);

            ReviewMessage =
                $"Reviewed PNG saved: {destinationPath}";
        }
        catch (Exception ex)
        {
            ReviewMessage =
                $"Reviewed PNG export failed: {ex.Message}";
        }
    }

    // ============================================================
    // IMAGE LOADING
    // ============================================================

    private static BitmapImage LoadBitmap(
        string filePath)
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

        return bitmap;
    }

    private static BitmapImage ConvertToBitmapImage(
        BitmapSource source)
    {
        var encoder =
            new PngBitmapEncoder();

        encoder.Frames.Add(
            BitmapFrame.Create(
                source));

        using var memory =
            new MemoryStream();

        encoder.Save(
            memory);

        memory.Position = 0;

        var bitmap =
            new BitmapImage();

        bitmap.BeginInit();

        bitmap.CacheOption =
            BitmapCacheOption.OnLoad;

        bitmap.StreamSource =
            memory;

        bitmap.EndInit();

        bitmap.Freeze();

        return bitmap;
    }

    // ============================================================
    // NAVIGATION
    // ============================================================

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

    // ============================================================
    // REVIEW STATUS
    // ============================================================

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

    // ============================================================
    // REVIEW HISTORY
    // ============================================================

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

    // ============================================================
    // OPEN IMAGE
    // ============================================================

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

    // ============================================================
    // IMAGE VIEWER EVENT
    // ============================================================

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

            SNR = 0;
            SNRText = "SNR: --";

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
        ResetImageFilter();

        UpdateNavigationState();

        UpdateReviewMessage();

        UpdateRuler();

        LoadReviewHistory();
    }

    // ============================================================
    // DISPLAY IMAGE
    // ============================================================

    private void LoadDisplayImage()
    {
        DisplayImage = null;

        if (_selectedImage == null)
        {
            SNR = 0;
            SNRText = "SNR: --";

            return;
        }

        string filePath =
            _selectedImage.FilePath;

        if (string.IsNullOrWhiteSpace(
                filePath))
        {
            SNR = 0;
            SNRText = "SNR: --";

            return;
        }

        if (!File.Exists(
                filePath))
        {
            SNR = 0;
            SNRText = "SNR: --";

            return;
        }

        try
        {
            var bitmap =
                LoadBitmap(
                    filePath);

            DisplayImage =
                bitmap;

            CalculateSNR(
                bitmap);
        }
        catch
        {
            DisplayImage = null;

            SNR = 0;
            SNRText = "SNR: --";
        }
    }

    // ============================================================
    // REVIEW MESSAGE
    // ============================================================

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

    // ============================================================
    // RULER
    // ============================================================

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

    // ============================================================
    // PROPERTY CHANGED
    // ============================================================

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

    // ============================================================
    // RULER TICK
    // ============================================================

    public sealed class RulerTick
    {
        public double Position { get; init; }

        public double RelativePosition { get; init; }

        public bool IsMajor { get; init; }

        public string Label { get; init; } =
            string.Empty;
    }
}