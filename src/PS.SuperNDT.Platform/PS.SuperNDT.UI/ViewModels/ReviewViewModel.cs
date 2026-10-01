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

    private double _brightness;
    private double _contrast;
    private double _gamma = 1.0;

    private double _snr;
    private string _snrText = "SNR: --";

    // RTR FILTERS
    private string _weldNumberFilter = string.Empty;
    private string _jointNumberFilter = string.Empty;
    private string _weldTypeFilter = string.Empty;
    private string _weldingProcessFilter = string.Empty;
    private string _iqiTypeFilter = string.Empty;
    private string _iqiSensitivityFilter = string.Empty;
    private string _filterFilter = string.Empty;
    private string _grainFilter = string.Empty;
    private string _defectTypeFilter = string.Empty;
    private string _acceptanceCodeFilter = string.Empty;
    private string _resultFilter = string.Empty;

    private string _snrMinFilter = string.Empty;
    private string _snrMaxFilter = string.Empty;
    private string _densityMinFilter = string.Empty;
    private string _densityMaxFilter = string.Empty;
    private string _contrastMinFilter = string.Empty;
    private string _contrastMaxFilter = string.Empty;
    private string _bsrMinFilter = string.Empty;
    private string _bsrMaxFilter = string.Empty;
    private string _kvMinFilter = string.Empty;
    private string _kvMaxFilter = string.Empty;
    private string _maMinFilter = string.Empty;
    private string _maMaxFilter = string.Empty;
    private string _exposureMinFilter = string.Empty;
    private string _exposureMaxFilter = string.Empty;
    private string _sfdMinFilter = string.Empty;
    private string _sfdMaxFilter = string.Empty;
    private string _oddMinFilter = string.Empty;
    private string _oddMaxFilter = string.Empty;
    private string _unsharpnessMinFilter = string.Empty;
    private string _unsharpnessMaxFilter = string.Empty;
    private string _materialThicknessMinFilter = string.Empty;
    private string _materialThicknessMaxFilter = string.Empty;
    private string _fromDateFilter = string.Empty;
    private string _toDateFilter = string.Empty;

    private bool _reviewedOnlyFilter;
    private bool _acceptedOnlyFilter;
    private bool _rejectedOnlyFilter;

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

    public RelayCommand ResetImageFilterCommand { get; }

    public RelayCommand ApplyImageFilterCommand { get; }

    public RelayCommand DeleteShotCommand { get; }

    public RelayCommand ApplyRtrFilterCommand { get; }

    public RelayCommand ClearRtrFilterCommand { get; }

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
    // RTR FILTER PROPERTIES
    // ============================================================

    public string WeldNumberFilter
    {
        get => _weldNumberFilter;
        set
        {
            value ??= string.Empty;
            if (_weldNumberFilter == value) return;
            _weldNumberFilter = value;
            OnPropertyChanged();
        }
    }

    public string JointNumberFilter
    {
        get => _jointNumberFilter;
        set
        {
            value ??= string.Empty;
            if (_jointNumberFilter == value) return;
            _jointNumberFilter = value;
            OnPropertyChanged();
        }
    }

    public string WeldTypeFilter
    {
        get => _weldTypeFilter;
        set
        {
            value ??= string.Empty;
            if (_weldTypeFilter == value) return;
            _weldTypeFilter = value;
            OnPropertyChanged();
        }
    }

    public string WeldingProcessFilter
    {
        get => _weldingProcessFilter;
        set
        {
            value ??= string.Empty;
            if (_weldingProcessFilter == value) return;
            _weldingProcessFilter = value;
            OnPropertyChanged();
        }
    }

    public string IqiTypeFilter
    {
        get => _iqiTypeFilter;
        set
        {
            value ??= string.Empty;
            if (_iqiTypeFilter == value) return;
            _iqiTypeFilter = value;
            OnPropertyChanged();
        }
    }

    public string IqiSensitivityFilter
    {
        get => _iqiSensitivityFilter;
        set
        {
            value ??= string.Empty;
            if (_iqiSensitivityFilter == value) return;
            _iqiSensitivityFilter = value;
            OnPropertyChanged();
        }
    }

    public string FilterFilter
    {
        get => _filterFilter;
        set
        {
            value ??= string.Empty;
            if (_filterFilter == value) return;
            _filterFilter = value;
            OnPropertyChanged();
        }
    }

    public string GrainFilter
    {
        get => _grainFilter;
        set
        {
            value ??= string.Empty;
            if (_grainFilter == value) return;
            _grainFilter = value;
            OnPropertyChanged();
        }
    }

    public string DefectTypeFilter
    {
        get => _defectTypeFilter;
        set
        {
            value ??= string.Empty;
            if (_defectTypeFilter == value) return;
            _defectTypeFilter = value;
            OnPropertyChanged();
        }
    }

    public string AcceptanceCodeFilter
    {
        get => _acceptanceCodeFilter;
        set
        {
            value ??= string.Empty;
            if (_acceptanceCodeFilter == value) return;
            _acceptanceCodeFilter = value;
            OnPropertyChanged();
        }
    }

    public string ResultFilter
    {
        get => _resultFilter;
        set
        {
            value ??= string.Empty;
            if (_resultFilter == value) return;
            _resultFilter = value;
            OnPropertyChanged();
        }
    }

    public string SnrMinFilter
    {
        get => _snrMinFilter;
        set
        {
            value ??= string.Empty;
            if (_snrMinFilter == value) return;
            _snrMinFilter = value;
            OnPropertyChanged();
        }
    }

    public string SnrMaxFilter
    {
        get => _snrMaxFilter;
        set
        {
            value ??= string.Empty;
            if (_snrMaxFilter == value) return;
            _snrMaxFilter = value;
            OnPropertyChanged();
        }
    }

    public string DensityMinFilter
    {
        get => _densityMinFilter;
        set
        {
            value ??= string.Empty;
            if (_densityMinFilter == value) return;
            _densityMinFilter = value;
            OnPropertyChanged();
        }
    }

    public string DensityMaxFilter
    {
        get => _densityMaxFilter;
        set
        {
            value ??= string.Empty;
            if (_densityMaxFilter == value) return;
            _densityMaxFilter = value;
            OnPropertyChanged();
        }
    }

    public string ContrastMinFilter
    {
        get => _contrastMinFilter;
        set
        {
            value ??= string.Empty;
            if (_contrastMinFilter == value) return;
            _contrastMinFilter = value;
            OnPropertyChanged();
        }
    }

    public string ContrastMaxFilter
    {
        get => _contrastMaxFilter;
        set
        {
            value ??= string.Empty;
            if (_contrastMaxFilter == value) return;
            _contrastMaxFilter = value;
            OnPropertyChanged();
        }
    }

    public string BsrMinFilter
    {
        get => _bsrMinFilter;
        set
        {
            value ??= string.Empty;
            if (_bsrMinFilter == value) return;
            _bsrMinFilter = value;
            OnPropertyChanged();
        }
    }

    public string BsrMaxFilter
    {
        get => _bsrMaxFilter;
        set
        {
            value ??= string.Empty;
            if (_bsrMaxFilter == value) return;
            _bsrMaxFilter = value;
            OnPropertyChanged();
        }
    }

    public string KvMinFilter
    {
        get => _kvMinFilter;
        set
        {
            value ??= string.Empty;
            if (_kvMinFilter == value) return;
            _kvMinFilter = value;
            OnPropertyChanged();
        }
    }

    public string KvMaxFilter
    {
        get => _kvMaxFilter;
        set
        {
            value ??= string.Empty;
            if (_kvMaxFilter == value) return;
            _kvMaxFilter = value;
            OnPropertyChanged();
        }
    }

    public string MaMinFilter
    {
        get => _maMinFilter;
        set
        {
            value ??= string.Empty;
            if (_maMinFilter == value) return;
            _maMinFilter = value;
            OnPropertyChanged();
        }
    }

    public string MaMaxFilter
    {
        get => _maMaxFilter;
        set
        {
            value ??= string.Empty;
            if (_maMaxFilter == value) return;
            _maMaxFilter = value;
            OnPropertyChanged();
        }
    }

    public string ExposureMinFilter
    {
        get => _exposureMinFilter;
        set
        {
            value ??= string.Empty;
            if (_exposureMinFilter == value) return;
            _exposureMinFilter = value;
            OnPropertyChanged();
        }
    }

    public string ExposureMaxFilter
    {
        get => _exposureMaxFilter;
        set
        {
            value ??= string.Empty;
            if (_exposureMaxFilter == value) return;
            _exposureMaxFilter = value;
            OnPropertyChanged();
        }
    }

    public string SfdMinFilter
    {
        get => _sfdMinFilter;
        set
        {
            value ??= string.Empty;
            if (_sfdMinFilter == value) return;
            _sfdMinFilter = value;
            OnPropertyChanged();
        }
    }

    public string SfdMaxFilter
    {
        get => _sfdMaxFilter;
        set
        {
            value ??= string.Empty;
            if (_sfdMaxFilter == value) return;
            _sfdMaxFilter = value;
            OnPropertyChanged();
        }
    }

    public string OddMinFilter
    {
        get => _oddMinFilter;
        set
        {
            value ??= string.Empty;
            if (_oddMinFilter == value) return;
            _oddMinFilter = value;
            OnPropertyChanged();
        }
    }

    public string OddMaxFilter
    {
        get => _oddMaxFilter;
        set
        {
            value ??= string.Empty;
            if (_oddMaxFilter == value) return;
            _oddMaxFilter = value;
            OnPropertyChanged();
        }
    }

    public string UnsharpnessMinFilter
    {
        get => _unsharpnessMinFilter;
        set
        {
            value ??= string.Empty;
            if (_unsharpnessMinFilter == value) return;
            _unsharpnessMinFilter = value;
            OnPropertyChanged();
        }
    }

    public string UnsharpnessMaxFilter
    {
        get => _unsharpnessMaxFilter;
        set
        {
            value ??= string.Empty;
            if (_unsharpnessMaxFilter == value) return;
            _unsharpnessMaxFilter = value;
            OnPropertyChanged();
        }
    }

    public string MaterialThicknessMinFilter
    {
        get => _materialThicknessMinFilter;
        set
        {
            value ??= string.Empty;
            if (_materialThicknessMinFilter == value) return;
            _materialThicknessMinFilter = value;
            OnPropertyChanged();
        }
    }

    public string MaterialThicknessMaxFilter
    {
        get => _materialThicknessMaxFilter;
        set
        {
            value ??= string.Empty;
            if (_materialThicknessMaxFilter == value) return;
            _materialThicknessMaxFilter = value;
            OnPropertyChanged();
        }
    }

    public string FromDateFilter
    {
        get => _fromDateFilter;
        set
        {
            value ??= string.Empty;
            if (_fromDateFilter == value) return;
            _fromDateFilter = value;
            OnPropertyChanged();
        }
    }

    public string ToDateFilter
    {
        get => _toDateFilter;
        set
        {
            value ??= string.Empty;
            if (_toDateFilter == value) return;
            _toDateFilter = value;
            OnPropertyChanged();
        }
    }

    public bool ReviewedOnlyFilter
    {
        get => _reviewedOnlyFilter;
        set
        {
            if (_reviewedOnlyFilter == value) return;
            _reviewedOnlyFilter = value;
            OnPropertyChanged();
            ApplyFilter();
        }
    }

    public bool AcceptedOnlyFilter
    {
        get => _acceptedOnlyFilter;
        set
        {
            if (_acceptedOnlyFilter == value) return;
            _acceptedOnlyFilter = value;
            OnPropertyChanged();
            ApplyFilter();
        }
    }

    public bool RejectedOnlyFilter
    {
        get => _rejectedOnlyFilter;
        set
        {
            if (_rejectedOnlyFilter == value) return;
            _rejectedOnlyFilter = value;
            OnPropertyChanged();
            ApplyFilter();
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

    private void ClearRtrFilters()
    {
        _weldNumberFilter = string.Empty;
        _jointNumberFilter = string.Empty;
        _weldTypeFilter = string.Empty;
        _weldingProcessFilter = string.Empty;
        _iqiTypeFilter = string.Empty;
        _iqiSensitivityFilter = string.Empty;
        _filterFilter = string.Empty;
        _grainFilter = string.Empty;
        _defectTypeFilter = string.Empty;
        _acceptanceCodeFilter = string.Empty;
        _resultFilter = string.Empty;
        _snrMinFilter = string.Empty; _snrMaxFilter = string.Empty;
        _densityMinFilter = string.Empty; _densityMaxFilter = string.Empty;
        _contrastMinFilter = string.Empty; _contrastMaxFilter = string.Empty;
        _bsrMinFilter = string.Empty; _bsrMaxFilter = string.Empty;
        _kvMinFilter = string.Empty; _kvMaxFilter = string.Empty;
        _maMinFilter = string.Empty; _maMaxFilter = string.Empty;
        _exposureMinFilter = string.Empty; _exposureMaxFilter = string.Empty;
        _sfdMinFilter = string.Empty; _sfdMaxFilter = string.Empty;
        _oddMinFilter = string.Empty; _oddMaxFilter = string.Empty;
        _unsharpnessMinFilter = string.Empty; _unsharpnessMaxFilter = string.Empty;
        _materialThicknessMinFilter = string.Empty; _materialThicknessMaxFilter = string.Empty;
        _fromDateFilter = string.Empty; _toDateFilter = string.Empty;
        _reviewedOnlyFilter = false; _acceptedOnlyFilter = false; _rejectedOnlyFilter = false;

        foreach (string name in new[]
        {
            nameof(WeldNumberFilter), nameof(JointNumberFilter), nameof(WeldTypeFilter),
            nameof(WeldingProcessFilter), nameof(IqiTypeFilter), nameof(IqiSensitivityFilter),
            nameof(FilterFilter), nameof(GrainFilter), nameof(DefectTypeFilter), nameof(AcceptanceCodeFilter),
            nameof(ResultFilter), nameof(SnrMinFilter), nameof(SnrMaxFilter), nameof(DensityMinFilter),
            nameof(DensityMaxFilter), nameof(ContrastMinFilter), nameof(ContrastMaxFilter), nameof(BsrMinFilter),
            nameof(BsrMaxFilter), nameof(KvMinFilter), nameof(KvMaxFilter), nameof(MaMinFilter), nameof(MaMaxFilter),
            nameof(ExposureMinFilter), nameof(ExposureMaxFilter), nameof(SfdMinFilter), nameof(SfdMaxFilter),
            nameof(OddMinFilter), nameof(OddMaxFilter), nameof(UnsharpnessMinFilter), nameof(UnsharpnessMaxFilter),
            nameof(MaterialThicknessMinFilter), nameof(MaterialThicknessMaxFilter), nameof(FromDateFilter), nameof(ToDateFilter),
            nameof(ReviewedOnlyFilter), nameof(AcceptedOnlyFilter), nameof(RejectedOnlyFilter)
        }) OnPropertyChanged(name);

        ApplyFilter();
    }

    private static bool RangeMatch(double value, string minText, string maxText)
    {
        if (double.TryParse(minText, out double min) && value < min) return false;
        if (double.TryParse(maxText, out double max) && value > max) return false;
        return true;
    }

    private static bool DateMatch(DateTime value, string fromText, string toText)
    {
        if (DateTime.TryParse(fromText, out DateTime from) && value.Date < from.Date) return false;
        if (DateTime.TryParse(toText, out DateTime to) && value.Date > to.Date) return false;
        return true;
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

        DeleteShotCommand =
            new RelayCommand(
                _ => DeleteSelectedShot());

        ApplyRtrFilterCommand =
            new RelayCommand(
                _ => ApplyFilter());

        ClearRtrFilterCommand =
            new RelayCommand(
                _ => ClearRtrFilters());

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

        Guid? previousSelectedId =
            _selectedImage?.Id;

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

                        if (!Contains(image.WeldNumber, WeldNumberFilter) ||
                            !Contains(image.JointNumber, JointNumberFilter) ||
                            !Contains(image.WeldType, WeldTypeFilter) ||
                            !Contains(image.WeldingProcess, WeldingProcessFilter) ||
                            !Contains(image.IQIType, IqiTypeFilter) ||
                            !Contains(image.IQISensitivity, IqiSensitivityFilter) ||
                            !Contains(image.Filter, FilterFilter) ||
                            !Contains(image.Grain, GrainFilter))
                        {
                            return false;
                        }

                        if (ReviewedOnlyFilter && !image.ReviewedOn.HasValue) return false;
                        if (AcceptedOnlyFilter && !string.Equals(image.ReviewStatus, "ACCEPTED", StringComparison.OrdinalIgnoreCase)) return false;
                        if (RejectedOnlyFilter && !string.Equals(image.ReviewStatus, "REJECTED", StringComparison.OrdinalIgnoreCase)) return false;

                        if (!RangeMatch(image.SNR, SnrMinFilter, SnrMaxFilter) ||
                            !RangeMatch(image.Density, DensityMinFilter, DensityMaxFilter) ||
                            !RangeMatch(image.Contrast, ContrastMinFilter, ContrastMaxFilter) ||
                            !RangeMatch(image.BasicSpatialResolution, BsrMinFilter, BsrMaxFilter) ||
                            !RangeMatch(image.KV, KvMinFilter, KvMaxFilter) ||
                            !RangeMatch(image.MA, MaMinFilter, MaMaxFilter) ||
                            !RangeMatch(image.ExposureTime, ExposureMinFilter, ExposureMaxFilter) ||
                            !RangeMatch(image.SFD, SfdMinFilter, SfdMaxFilter) ||
                            !RangeMatch(image.ODD, OddMinFilter, OddMaxFilter) ||
                            !RangeMatch(image.GeometricUnsharpness, UnsharpnessMinFilter, UnsharpnessMaxFilter) ||
                            !RangeMatch(image.MaterialThickness, MaterialThicknessMinFilter, MaterialThicknessMaxFilter))
                        {
                            return false;
                        }

                        if (!DateMatch(image.CapturedOn, FromDateFilter, ToDateFilter))
                        {
                            return false;
                        }

                        // DefectType / AcceptanceCode / Result are retained as filter inputs.
                        // They will become active when defect-review records expose those fields.

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

        /*
         * IMPORTANT:
         * Never automatically move the user to Shot 1
         * just because the current shot was updated.
         */
        if (previousSelectedId.HasValue)
        {
            var sameImage =
                FilteredImages.FirstOrDefault(
                    image =>
                        image.Id ==
                        previousSelectedId.Value);

            if (sameImage != null)
            {
                if (!ReferenceEquals(
                        _selectedImage,
                        sameImage))
                {
                    SelectedImage =
                        sameImage;
                }
            }
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
            /*
             * Keep the exact shot ID before doing anything.
             * We will reselect this same shot at the end.
             */
            Guid selectedImageId =
                _selectedImage.Id;

            int selectedShotNumber =
                _selectedImage.ShotNumber;

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

            // ----------------------------------------------------
            // 1. Get defects before moving the clean image.
            // ----------------------------------------------------

            var defects =
                DefectService.Instance
                    .GetByImage(
                        selectedImageId)
                    .ToList();

            // ----------------------------------------------------
            // 2. Move CLEAN ORIGINAL image to status folder.
            //
            // IMPORTANT:
            // We do NOT replace this image with reviewed PNG.
            // ----------------------------------------------------

            string movedFilePath =
                oldFilePath;

            if (!string.IsNullOrWhiteSpace(
                    folderStatus) &&
                !string.IsNullOrWhiteSpace(
                    oldFilePath) &&
                File.Exists(oldFilePath))
            {
                movedFilePath =
                    _imageFolderService
                        .MoveImageToStatus(
                            _selectedImage,
                            folderStatus);

                if (string.IsNullOrWhiteSpace(
                        movedFilePath))
                {
                    throw new InvalidOperationException(
                        "The image could not be moved to the selected status folder.");
                }

                if (!File.Exists(
                        movedFilePath))
                {
                    throw new FileNotFoundException(
                        "The moved image was not found.",
                        movedFilePath);
                }

                _selectedImage.FilePath =
                    movedFilePath;
            }

            // ----------------------------------------------------
            // 3. Export MARKED image separately.
            //
            // Clean image remains untouched.
            //
            // Example:
            //
            // ACCEPT\
            //     IMG_S001_xxx.png
            //
            //     Reviewed\
            //         IMG_S001_xxx_REVIEWED.png
            //
            // ----------------------------------------------------

            string? reviewedPath = null;

            if (!string.IsNullOrWhiteSpace(
                    _selectedImage.FilePath) &&
                File.Exists(
                    _selectedImage.FilePath) &&
                DisplayImage != null)
            {
                reviewedPath =
                    _reviewedImageExportService
                        .ExportReviewedPng(
                            _selectedImage,
                            DisplayImage,
                            defects);
            }

            // ----------------------------------------------------
            // 4. Save review status.
            // ----------------------------------------------------

            _selectedImage.ReviewStatus =
                status;

            _selectedImage.ReviewedBy =
                reviewer;

            _selectedImage.ReviewedOn =
                reviewTime;

            _imageService.Save(
                _selectedImage);

            // ----------------------------------------------------
            // 5. Audit.
            // ----------------------------------------------------

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
                    $"Reviewed On: {reviewTime:yyyy-MM-dd HH:mm:ss}" +
                    (string.IsNullOrWhiteSpace(reviewedPath)
                        ? string.Empty
                        : $" | Reviewed PNG: {reviewedPath}"));
            }
            catch
            {
                // Audit failure must not prevent review status save.
            }

            // ----------------------------------------------------
            // 6. Update the existing object only.
            //
            // DO NOT call ApplyFilter() here.
            // That was causing the selected shot to jump.
            // ----------------------------------------------------

            OnPropertyChanged(
                nameof(SelectedImage));

            OnPropertyChanged(
                nameof(PendingImages));

            OnPropertyChanged(
                nameof(AcceptedImages));

            OnPropertyChanged(
                nameof(RejectedImages));

            // ----------------------------------------------------
            // 7. Reload CLEAN image from its new status path.
            // ----------------------------------------------------

            LoadDisplayImage();

            LoadReviewHistory();

            UpdateNavigationState();

            UpdateRuler();

            ImageViewerService.Instance.OpenImage(
                _selectedImage);

            if (string.IsNullOrWhiteSpace(
                    folderStatus))
            {
                ReviewMessage =
                    $"Shot {selectedShotNumber} marked {status}.";
            }
            else if (!string.IsNullOrWhiteSpace(
                         reviewedPath))
            {
                ReviewMessage =
                    $"Shot {selectedShotNumber} marked {status}. " +
                    $"Clean image moved to {folderStatus}; " +
                    $"marked copy saved in Reviewed.";
            }
            else
            {
                ReviewMessage =
                    $"Shot {selectedShotNumber} marked {status} " +
                    $"and moved to {folderStatus}.";
            }

            /*
             * Keep the exact same shot selected.
             */
            var sameShot =
                Images.FirstOrDefault(
                    image =>
                        image.Id ==
                        selectedImageId);

            if (sameShot != null &&
                !ReferenceEquals(
                    _selectedImage,
                    sameShot))
            {
                SelectedImage =
                    sameShot;
            }
        }
        catch (Exception ex)
        {
            ReviewMessage =
                $"Review update failed: {ex.Message}";
        }
    }

    // ============================================================
    // DELETE SHOT
    // ============================================================

    private void DeleteSelectedShot()
    {
        if (_selectedImage == null)
        {
            ReviewMessage =
                "Select a shot first.";

            return;
        }

        ImageRecordModel imageToDelete =
            _selectedImage;

        Guid imageId =
            imageToDelete.Id;

        int shotNumber =
            imageToDelete.ShotNumber;

        string jobNumber =
            imageToDelete.JobNumber ?? string.Empty;

        var result =
            MessageBox.Show(
                $"Delete Shot {shotNumber} permanently?\n\n" +
                $"Work Order: {jobNumber}\n" +
                $"Shot: {shotNumber}\n\n" +
                "This will remove the shot from Review, " +
                "its saved defects, and its reviewed PNG.",
                "Delete Shot",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            // ----------------------------------------------------
            // 1. Remove all defect records for this image.
            // ----------------------------------------------------

            DefectService.Instance.ClearImage(
                imageId);

            // ----------------------------------------------------
            // 2. Delete the clean image file.
            // ----------------------------------------------------

            DeleteImageFile(
                imageToDelete.FilePath);

            // ----------------------------------------------------
            // 3. Delete all REVIEWED copies belonging to this image.
            // ----------------------------------------------------

            DeleteReviewedFiles(
                imageId);

            // ----------------------------------------------------
            // 4. Delete database image record.
            // ----------------------------------------------------

            _imageService.Delete(
                imageId);

            // ----------------------------------------------------
            // 5. Remove from in-memory collections.
            // ----------------------------------------------------

            var imageFromImages =
                Images.FirstOrDefault(
                    image =>
                        image.Id ==
                        imageId);

            if (imageFromImages != null)
            {
                Images.Remove(
                    imageFromImages);
            }

            var imageFromFiltered =
                FilteredImages.FirstOrDefault(
                    image =>
                        image.Id ==
                        imageId);

            if (imageFromFiltered != null)
            {
                FilteredImages.Remove(
                    imageFromFiltered);
            }

            // ----------------------------------------------------
            // 6. Clear current viewer state.
            // ----------------------------------------------------

            _selectedImage = null;

            DisplayImage = null;

            ImageViewerService.Instance.Clear();

            RulerTicks.Clear();
            ReviewHistory.Clear();

            SNR = 0;
            SNRText = "SNR: --";

            OnPropertyChanged(
                nameof(SelectedImage));

            OnPropertyChanged(
                nameof(TotalImages));

            OnPropertyChanged(
                nameof(PendingImages));

            OnPropertyChanged(
                nameof(AcceptedImages));

            OnPropertyChanged(
                nameof(RejectedImages));

            // ----------------------------------------------------
            // 7. Select another shot only AFTER deletion.
            //
            // This is intentional. Delete is the only operation
            // where automatic selection of another shot is allowed.
            // ----------------------------------------------------

            if (FilteredImages.Count > 0)
            {
                SelectedImage =
                    FilteredImages[0];
            }
            else if (Images.Count > 0)
            {
                var nextImage =
                    Images
                        .OrderBy(
                            image => image.CapturedOn)
                        .ThenBy(
                            image => image.JobNumber)
                        .ThenBy(
                            image => image.ShotNumber)
                        .FirstOrDefault();

                if (nextImage != null)
                {
                    SelectedImage =
                        nextImage;
                }
            }

            UpdateNavigationState();
            UpdateRuler();

            ReviewMessage =
                $"Shot {shotNumber} deleted successfully.";
        }
        catch (Exception ex)
        {
            ReviewMessage =
                $"Shot delete failed: {ex.Message}";
        }
    }

    private static void DeleteImageFile(
        string? filePath)
    {
        if (string.IsNullOrWhiteSpace(
                filePath))
        {
            return;
        }

        try
        {
            if (File.Exists(
                    filePath))
            {
                File.Delete(
                    filePath);
            }
        }
        catch
        {
            // Continue with database cleanup.
        }
    }

    private static void DeleteReviewedFiles(
        Guid imageId)
    {
        try
        {
            string jobsRoot =
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Jobs");

            if (!Directory.Exists(
                    jobsRoot))
            {
                return;
            }

            string idText =
                imageId.ToString("N");

            var reviewedFiles =
                Directory.GetFiles(
                    jobsRoot,
                    "*_REVIEWED.png",
                    SearchOption.AllDirectories)
                .Where(
                    file =>
                        Path.GetFileName(
                            file)
                        .Contains(
                            idText,
                            StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (string reviewedFile
                     in reviewedFiles)
            {
                try
                {
                    if (File.Exists(
                            reviewedFile))
                    {
                        File.Delete(
                            reviewedFile);
                    }
                }
                catch
                {
                    // Continue deleting remaining files.
                }
            }
        }
        catch
        {
            // Reviewed-file cleanup must not stop DB deletion.
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