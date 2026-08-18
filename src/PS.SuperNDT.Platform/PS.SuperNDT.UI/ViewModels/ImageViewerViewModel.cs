using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PS.SuperNDT.UI.Commands;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ImageViewerViewModel : INotifyPropertyChanged, IDisposable
{
    private ImageRecordModel? _currentImage;

    private double _zoomScale = 1.0;
    private double _rotationAngle;

    private double _brightness;
    private double _contrast = 1.0;
    private bool _isInverted;

    private bool _isMeasurementMode;

    private Point? _measurementStartPoint;
    private Point? _measurementEndPoint;

    private bool _isCalibrationMode;

    private Point? _calibrationStartPoint;
    private Point? _calibrationEndPoint;

    private double _calibrationReferenceLengthMm;

    private double _calibrationMmPerPixel = 1.0;

    private bool _disposed;

    public ImageRecordModel? CurrentImage
    {
        get => _currentImage;
        private set
        {
            if (ReferenceEquals(
                    _currentImage,
                    value))
            {
                return;
            }

            _currentImage = value;

            OnPropertyChanged();
            OnPropertyChanged(nameof(ImageInfo));
            OnPropertyChanged(nameof(DisplayImage));

            ApplyImageAdjustments();

            ClearMeasurement();
            ClearCalibrationPoints();
        }
    }

    public double ZoomScale
    {
        get => _zoomScale;

        private set
        {
            if (Math.Abs(
                    _zoomScale -
                    value) < 0.001)
            {
                return;
            }

            _zoomScale = value;

            OnPropertyChanged();
            OnPropertyChanged(nameof(ZoomText));
        }
    }

    public string ZoomText =>
        $"{ZoomScale * 100:0}%";

    public double RotationAngle
    {
        get => _rotationAngle;

        private set
        {
            if (Math.Abs(
                    _rotationAngle -
                    value) < 0.001)
            {
                return;
            }

            _rotationAngle = value;

            OnPropertyChanged();
            OnPropertyChanged(nameof(RotationText));
        }
    }

    public string RotationText =>
        $"{RotationAngle:0}°";

    public double Brightness
    {
        get => _brightness;

        set
        {
            double valueClamped =
                Math.Clamp(
                    value,
                    -100.0,
                    100.0);

            if (Math.Abs(
                    _brightness -
                    valueClamped) < 0.001)
            {
                return;
            }

            _brightness =
                valueClamped;

            OnPropertyChanged();

            ApplyImageAdjustments();
        }
    }

    public double Contrast
    {
        get => _contrast;

        set
        {
            double valueClamped =
                Math.Clamp(
                    value,
                    0.0,
                    2.0);

            if (Math.Abs(
                    _contrast -
                    valueClamped) < 0.001)
            {
                return;
            }

            _contrast =
                valueClamped;

            OnPropertyChanged();

            ApplyImageAdjustments();
        }
    }

    public bool IsInverted
    {
        get => _isInverted;

        private set
        {
            if (_isInverted == value)
                return;

            _isInverted =
                value;

            OnPropertyChanged();

            ApplyImageAdjustments();
        }
    }

    public ImageSource? DisplayImage { get; private set; }

    // ------------------------------------------------------------
    // MEASUREMENT
    // ------------------------------------------------------------

    public bool IsMeasurementMode
    {
        get => _isMeasurementMode;

        private set
        {
            if (_isMeasurementMode == value)
                return;

            _isMeasurementMode =
                value;

            OnPropertyChanged();
            OnPropertyChanged(
                nameof(MeasurementStatus));
        }
    }

    public Point? MeasurementStartPoint
    {
        get => _measurementStartPoint;

        private set
        {
            if (_measurementStartPoint == value)
                return;

            _measurementStartPoint =
                value;

            OnPropertyChanged();

            OnPropertyChanged(
                nameof(HasMeasurement));

            OnPropertyChanged(
                nameof(MeasurementStatus));

            OnPropertyChanged(
                nameof(MeasurementDistancePixels));

            OnPropertyChanged(
                nameof(MeasurementDistanceMm));

            OnPropertyChanged(
                nameof(MeasurementText));
        }
    }

    public Point? MeasurementEndPoint
    {
        get => _measurementEndPoint;

        private set
        {
            if (_measurementEndPoint == value)
                return;

            _measurementEndPoint =
                value;

            OnPropertyChanged();

            OnPropertyChanged(
                nameof(HasMeasurement));

            OnPropertyChanged(
                nameof(MeasurementStatus));

            OnPropertyChanged(
                nameof(MeasurementDistancePixels));

            OnPropertyChanged(
                nameof(MeasurementDistanceMm));

            OnPropertyChanged(
                nameof(MeasurementText));
        }
    }

    public bool HasMeasurement =>
        MeasurementStartPoint.HasValue &&
        MeasurementEndPoint.HasValue;

    public double MeasurementDistancePixels
    {
        get
        {
            if (!HasMeasurement)
                return 0;

            Point start =
                MeasurementStartPoint!.Value;

            Point end =
                MeasurementEndPoint!.Value;

            double dx =
                end.X - start.X;

            double dy =
                end.Y - start.Y;

            return Math.Sqrt(
                (dx * dx) +
                (dy * dy));
        }
    }

    // ------------------------------------------------------------
    // CALIBRATION
    // ------------------------------------------------------------

    public bool IsCalibrationMode
    {
        get => _isCalibrationMode;

        private set
        {
            if (_isCalibrationMode == value)
                return;

            _isCalibrationMode =
                value;

            OnPropertyChanged();

            OnPropertyChanged(
                nameof(CalibrationStatus));
        }
    }

    public Point? CalibrationStartPoint
    {
        get => _calibrationStartPoint;

        private set
        {
            if (_calibrationStartPoint == value)
                return;

            _calibrationStartPoint =
                value;

            OnPropertyChanged();

            OnPropertyChanged(
                nameof(HasCalibrationPoints));

            OnPropertyChanged(
                nameof(CalibrationPixelDistance));

            OnPropertyChanged(
                nameof(CalibrationStatus));
        }
    }

    public Point? CalibrationEndPoint
    {
        get => _calibrationEndPoint;

        private set
        {
            if (_calibrationEndPoint == value)
                return;

            _calibrationEndPoint =
                value;

            OnPropertyChanged();

            OnPropertyChanged(
                nameof(HasCalibrationPoints));

            OnPropertyChanged(
                nameof(CalibrationPixelDistance));

            OnPropertyChanged(
                nameof(CalibrationStatus));
        }
    }

    public bool HasCalibrationPoints =>
        CalibrationStartPoint.HasValue &&
        CalibrationEndPoint.HasValue;

    public double CalibrationPixelDistance
    {
        get
        {
            if (!HasCalibrationPoints)
                return 0;

            Point start =
                CalibrationStartPoint!.Value;

            Point end =
                CalibrationEndPoint!.Value;

            double dx =
                end.X - start.X;

            double dy =
                end.Y - start.Y;

            return Math.Sqrt(
                (dx * dx) +
                (dy * dy));
        }
    }

    public double CalibrationReferenceLengthMm
    {
        get => _calibrationReferenceLengthMm;

        set
        {
            double valueClamped =
                Math.Max(
                    0,
                    value);

            if (Math.Abs(
                    _calibrationReferenceLengthMm -
                    valueClamped) < 0.000001)
            {
                return;
            }

            _calibrationReferenceLengthMm =
                valueClamped;

            OnPropertyChanged();

            OnPropertyChanged(
                nameof(CalibrationStatus));
        }
    }

    public double CalibrationMmPerPixel
    {
        get => _calibrationMmPerPixel;

        private set
        {
            double valueClamped =
                Math.Clamp(
                    value,
                    0.000001,
                    1000.0);

            if (Math.Abs(
                    _calibrationMmPerPixel -
                    valueClamped) < 0.000001)
            {
                return;
            }

            _calibrationMmPerPixel =
                valueClamped;

            OnPropertyChanged();

            OnPropertyChanged(
                nameof(MeasurementDistanceMm));

            OnPropertyChanged(
                nameof(MeasurementText));

            OnPropertyChanged(
                nameof(CalibrationStatus));
        }
    }

    public double MeasurementDistanceMm =>
        MeasurementDistancePixels *
        CalibrationMmPerPixel;

    public string MeasurementText
    {
        get
        {
            if (!HasMeasurement)
                return "No Measurement";

            return
                $"Distance: " +
                $"{MeasurementDistanceMm:0.00} mm  " +
                $"({MeasurementDistancePixels:0.00} px)";
        }
    }

    public string MeasurementStatus
    {
        get
        {
            if (!IsMeasurementMode)
                return "Measurement Off";

            if (!MeasurementStartPoint.HasValue)
                return "Click first point";

            if (!MeasurementEndPoint.HasValue)
                return "Click second point";

            return MeasurementText;
        }
    }

    public string CalibrationStatus
    {
        get
        {
            if (!IsCalibrationMode)
                return
                    $"Calibration: " +
                    $"{CalibrationMmPerPixel:0.000000} mm/px";

            if (!CalibrationStartPoint.HasValue)
                return "Calibration: click first point";

            if (!CalibrationEndPoint.HasValue)
                return "Calibration: click second point";

            if (CalibrationReferenceLengthMm <= 0)
                return "Enter reference length in mm";

            if (CalibrationPixelDistance <= 0)
                return "Invalid calibration distance";

            return
                $"Calibration ready: " +
                $"{CalibrationPixelDistance:0.00} px → " +
                $"{CalibrationReferenceLengthMm:0.00} mm";
        }
    }

    // ------------------------------------------------------------
    // IMAGE INFORMATION
    // ------------------------------------------------------------

    public string ImageInfo
    {
        get
        {
            if (CurrentImage == null)
                return "No Image Selected";

            return
                $"Frame : {CurrentImage.FrameNumber}\n" +
                $"Size : " +
                $"{CurrentImage.ImageWidth} x " +
                $"{CurrentImage.ImageHeight}\n" +
                $"Bit Depth : {CurrentImage.BitDepth}\n" +
                $"kV : {CurrentImage.KV}\n" +
                $"mA : {CurrentImage.MA}\n" +
                $"Exposure : " +
                $"{CurrentImage.ExposureTime}";
        }
    }

    // ------------------------------------------------------------
    // COMMANDS
    // ------------------------------------------------------------

    public ICommand ZoomInCommand { get; }

    public ICommand ZoomOutCommand { get; }

    public ICommand FitToWindowCommand { get; }

    public ICommand ActualSizeCommand { get; }

    public ICommand RotateLeftCommand { get; }

    public ICommand RotateRightCommand { get; }

    public ICommand IncreaseBrightnessCommand { get; }

    public ICommand DecreaseBrightnessCommand { get; }

    public ICommand IncreaseContrastCommand { get; }

    public ICommand DecreaseContrastCommand { get; }

    public ICommand ToggleInvertCommand { get; }

    public ICommand ResetImageAdjustmentsCommand { get; }

    public ICommand ToggleMeasurementCommand { get; }

    public ICommand ClearMeasurementCommand { get; }

    public ICommand ToggleCalibrationCommand { get; }

    public ICommand ClearCalibrationCommand { get; }

    public ICommand ApplyCalibrationCommand { get; }

    public ImageViewerViewModel()
    {
        ZoomInCommand =
            new RelayCommand(
                _ => ZoomIn());

        ZoomOutCommand =
            new RelayCommand(
                _ => ZoomOut());

        FitToWindowCommand =
            new RelayCommand(
                _ => FitToWindow());

        ActualSizeCommand =
            new RelayCommand(
                _ => ActualSize());

        RotateLeftCommand =
            new RelayCommand(
                _ => RotateLeft());

        RotateRightCommand =
            new RelayCommand(
                _ => RotateRight());

        IncreaseBrightnessCommand =
            new RelayCommand(
                _ => Brightness += 10);

        DecreaseBrightnessCommand =
            new RelayCommand(
                _ => Brightness -= 10);

        IncreaseContrastCommand =
            new RelayCommand(
                _ => Contrast += 0.10);

        DecreaseContrastCommand =
            new RelayCommand(
                _ => Contrast -= 0.10);

        ToggleInvertCommand =
            new RelayCommand(
                _ => IsInverted = !IsInverted);

        ResetImageAdjustmentsCommand =
            new RelayCommand(
                _ => ResetImageAdjustments());

        ToggleMeasurementCommand =
            new RelayCommand(
                _ => ToggleMeasurement());

        ClearMeasurementCommand =
            new RelayCommand(
                _ => ClearMeasurement());

        ToggleCalibrationCommand =
            new RelayCommand(
                _ => ToggleCalibration());

        ClearCalibrationCommand =
            new RelayCommand(
                _ => ClearCalibrationPoints());

        ApplyCalibrationCommand =
            new RelayCommand(
                _ => ApplyCalibration());

        ImageViewerService.Instance.CurrentImageChanged +=
            ImageViewerService_CurrentImageChanged;

        CurrentImage =
            ImageViewerService.Instance.CurrentImage;
    }

    // ------------------------------------------------------------
    // VIEW CONTROLS
    // ------------------------------------------------------------

    private void ZoomIn()
    {
        ZoomScale =
            Math.Min(
                5.0,
                Math.Round(
                    ZoomScale + 0.10,
                    2));
    }

    private void ZoomOut()
    {
        ZoomScale =
            Math.Max(
                0.10,
                Math.Round(
                    ZoomScale - 0.10,
                    2));
    }

    private void FitToWindow()
    {
        ZoomScale = 1.0;
    }

    private void ActualSize()
    {
        ZoomScale = 1.0;
    }

    private void RotateLeft()
    {
        RotationAngle -= 90;

        if (RotationAngle < 0)
            RotationAngle += 360;
    }

    private void RotateRight()
    {
        RotationAngle += 90;

        if (RotationAngle >= 360)
            RotationAngle -= 360;
    }

    private void ResetImageAdjustments()
    {
        Brightness = 0;
        Contrast = 1.0;
        IsInverted = false;
    }

    // ------------------------------------------------------------
    // MEASUREMENT
    // ------------------------------------------------------------

    private void ToggleMeasurement()
    {
        IsMeasurementMode =
            !IsMeasurementMode;

        if (!IsMeasurementMode)
            ClearMeasurement();
    }

    public void SetMeasurementStartPoint(
        Point point)
    {
        if (!IsMeasurementMode)
            return;

        MeasurementStartPoint =
            point;

        MeasurementEndPoint =
            null;

        OnPropertyChanged(
            nameof(MeasurementDistancePixels));

        OnPropertyChanged(
            nameof(MeasurementDistanceMm));

        OnPropertyChanged(
            nameof(MeasurementText));

        OnPropertyChanged(
            nameof(MeasurementStatus));
    }

    public void SetMeasurementEndPoint(
        Point point)
    {
        if (!IsMeasurementMode)
            return;

        if (!MeasurementStartPoint.HasValue)
        {
            SetMeasurementStartPoint(point);
            return;
        }

        MeasurementEndPoint =
            point;

        OnPropertyChanged(
            nameof(MeasurementDistancePixels));

        OnPropertyChanged(
            nameof(MeasurementDistanceMm));

        OnPropertyChanged(
            nameof(MeasurementText));

        OnPropertyChanged(
            nameof(MeasurementStatus));
    }

    private void ClearMeasurement()
    {
        MeasurementStartPoint = null;
        MeasurementEndPoint = null;

        OnPropertyChanged(
            nameof(MeasurementDistancePixels));

        OnPropertyChanged(
            nameof(MeasurementDistanceMm));

        OnPropertyChanged(
            nameof(MeasurementText));

        OnPropertyChanged(
            nameof(MeasurementStatus));
    }

    // ------------------------------------------------------------
    // CALIBRATION
    // ------------------------------------------------------------

    private void ToggleCalibration()
    {
        IsCalibrationMode =
            !IsCalibrationMode;

        if (!IsCalibrationMode)
            ClearCalibrationPoints();
    }

    public void SetCalibrationStartPoint(
        Point point)
    {
        if (!IsCalibrationMode)
            return;

        CalibrationStartPoint =
            point;

        CalibrationEndPoint =
            null;

        OnPropertyChanged(
            nameof(CalibrationPixelDistance));

        OnPropertyChanged(
            nameof(CalibrationStatus));
    }

    public void SetCalibrationEndPoint(
        Point point)
    {
        if (!IsCalibrationMode)
            return;

        if (!CalibrationStartPoint.HasValue)
        {
            SetCalibrationStartPoint(point);
            return;
        }

        CalibrationEndPoint =
            point;

        OnPropertyChanged(
            nameof(CalibrationPixelDistance));

        OnPropertyChanged(
            nameof(CalibrationStatus));
    }

    private void ClearCalibrationPoints()
    {
        CalibrationStartPoint = null;
        CalibrationEndPoint = null;

        OnPropertyChanged(
            nameof(CalibrationPixelDistance));

        OnPropertyChanged(
            nameof(CalibrationStatus));
    }

    private void ApplyCalibration()
    {
        if (!HasCalibrationPoints)
            return;

        if (CalibrationReferenceLengthMm <= 0)
            return;

        double pixelDistance =
            CalibrationPixelDistance;

        if (pixelDistance <= 0)
            return;

        CalibrationMmPerPixel =
            CalibrationReferenceLengthMm /
            pixelDistance;

        IsCalibrationMode = false;

        ClearCalibrationPoints();
    }

    // ------------------------------------------------------------
    // IMAGE PROCESSING
    // ------------------------------------------------------------

    private void ApplyImageAdjustments()
    {
        if (CurrentImage == null ||
            string.IsNullOrWhiteSpace(
                CurrentImage.FilePath))
        {
            DisplayImage = null;

            OnPropertyChanged(
                nameof(DisplayImage));

            return;
        }

        try
        {
            var bitmap =
                new BitmapImage(
                    new Uri(
                        CurrentImage.FilePath,
                        UriKind.Absolute));

            bitmap.CacheOption =
                BitmapCacheOption.OnLoad;

            bitmap.Freeze();

            var converted =
                new FormatConvertedBitmap(
                    bitmap,
                    PixelFormats.Bgra32,
                    null,
                    0);

            converted.Freeze();

            int width =
                converted.PixelWidth;

            int height =
                converted.PixelHeight;

            int stride =
                width * 4;

            byte[] pixels =
                new byte[stride * height];

            converted.CopyPixels(
                pixels,
                stride,
                0);

            double contrastFactor =
                Contrast;

            double brightnessOffset =
                Brightness * 2.55;

            for (
                int i = 0;
                i < pixels.Length;
                i += 4)
            {
                double blue =
                    pixels[i];

                double green =
                    pixels[i + 1];

                double red =
                    pixels[i + 2];

                red =
                    ((red - 127.5) *
                     contrastFactor)
                    + 127.5
                    + brightnessOffset;

                green =
                    ((green - 127.5) *
                     contrastFactor)
                    + 127.5
                    + brightnessOffset;

                blue =
                    ((blue - 127.5) *
                     contrastFactor)
                    + 127.5
                    + brightnessOffset;

                if (IsInverted)
                {
                    red = 255 - red;
                    green = 255 - green;
                    blue = 255 - blue;
                }

                pixels[i] =
                    ClampToByte(blue);

                pixels[i + 1] =
                    ClampToByte(green);

                pixels[i + 2] =
                    ClampToByte(red);
            }

            var result =
                new WriteableBitmap(
                    width,
                    height,
                    converted.DpiX,
                    converted.DpiY,
                    PixelFormats.Bgra32,
                    null);

            result.WritePixels(
                new Int32Rect(
                    0,
                    0,
                    width,
                    height),
                pixels,
                stride,
                0);

            result.Freeze();

            DisplayImage =
                result;

            OnPropertyChanged(
                nameof(DisplayImage));
        }
        catch
        {
            DisplayImage = null;

            OnPropertyChanged(
                nameof(DisplayImage));
        }
    }

    private static byte ClampToByte(
        double value)
    {
        return (byte)Math.Clamp(
            (int)Math.Round(value),
            0,
            255);
    }

    // ------------------------------------------------------------
    // SERVICE
    // ------------------------------------------------------------

    private void ImageViewerService_CurrentImageChanged(
        object? sender,
        EventArgs e)
    {
        if (_disposed)
            return;

        CurrentImage =
            ImageViewerService.Instance.CurrentImage;

        Reset();
    }

    private void Reset()
    {
        ZoomScale = 1.0;

        RotationAngle = 0;

        ResetImageAdjustments();

        IsMeasurementMode = false;

        IsCalibrationMode = false;

        ClearMeasurement();

        ClearCalibrationPoints();
    }

    // ------------------------------------------------------------
    // DISPOSE
    // ------------------------------------------------------------

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        ImageViewerService.Instance.CurrentImageChanged -=
            ImageViewerService_CurrentImageChanged;

        GC.SuppressFinalize(this);
    }

    // ------------------------------------------------------------
    // PROPERTY CHANGED
    // ------------------------------------------------------------

    public event PropertyChangedEventHandler?
        PropertyChanged;

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