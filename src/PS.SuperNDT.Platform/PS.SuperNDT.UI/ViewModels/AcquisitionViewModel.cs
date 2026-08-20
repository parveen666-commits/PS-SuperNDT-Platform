using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PS.SuperNDT.UI.Commands;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class AcquisitionViewModel : INotifyPropertyChanged
{
    private readonly ImageService _imageService = new();

    private string _detectorStatus = "Offline";
    private string _connectionStatus = "Disconnected";
    private string _currentJob = "No Active Job";
    private string _acquisitionStatus = "Ready";
    private string _pipeId = "";

    private int _frameNumber;
    private double _kv = 120;
    private double _ma = 5;
    private double _exposureTime = 2;

    private double _pipeLength = 6000;
    private double _shotSize = 300;
    private double _overlap = 10;

    private int _currentShotNumber = 1;

    private ImageRecordModel? _capturedImage;
    private ShotPlanModel? _shotPlan;
    private ImageSource? _capturedImageSource;

    public RelayCommand ConnectCommand { get; }

    public RelayCommand CaptureCommand { get; }

    public RelayCommand SaveCommand { get; }

    public RelayCommand RetakeCommand { get; }

    public RelayCommand WindowLevelCommand { get; }

    public RelayCommand ZoomCommand { get; }

    public string DetectorStatus
    {
        get => _detectorStatus;
        set
        {
            _detectorStatus = value;
            OnPropertyChanged();
        }
    }

    public string ConnectionStatus
    {
        get => _connectionStatus;
        set
        {
            _connectionStatus = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsConnected));
        }
    }

    public string CurrentJob
    {
        get => _currentJob;
        set
        {
            _currentJob = value;
            OnPropertyChanged();
        }
    }

    public string PipeId
    {
        get => _pipeId;
        set
        {
            value ??= "";

            if (_pipeId == value)
            {
                return;
            }

            _pipeId = value;

            OnPropertyChanged();

            RebuildShotPlan();
        }
    }

    public string AcquisitionStatus
    {
        get => _acquisitionStatus;
        set
        {
            _acquisitionStatus = value;
            OnPropertyChanged();
        }
    }

    public int FrameNumber
    {
        get => _frameNumber;
        set
        {
            _frameNumber = value;
            OnPropertyChanged();
        }
    }

    public double KV
    {
        get => _kv;
        set
        {
            _kv = value;
            OnPropertyChanged();
        }
    }

    public double MA
    {
        get => _ma;
        set
        {
            _ma = value;
            OnPropertyChanged();
        }
    }

    public double ExposureTime
    {
        get => _exposureTime;
        set
        {
            _exposureTime = value;
            OnPropertyChanged();
        }
    }

    public double PipeLength
    {
        get => _pipeLength;
        set
        {
            if (value < 0)
            {
                value = 0;
            }

            if (Math.Abs(_pipeLength - value) < 0.001)
            {
                return;
            }

            _pipeLength = value;

            OnPropertyChanged();

            RebuildShotPlan();
        }
    }

    public double ShotSize
    {
        get => _shotSize;
        set
        {
            if (value < 1)
            {
                value = 1;
            }

            if (Math.Abs(_shotSize - value) < 0.001)
            {
                return;
            }

            _shotSize = value;

            OnPropertyChanged();

            RebuildShotPlan();
        }
    }

    public double Overlap
    {
        get => _overlap;
        set
        {
            if (value < 0)
            {
                value = 0;
            }

            if (value >= ShotSize)
            {
                value = Math.Max(0, ShotSize - 1);
            }

            if (Math.Abs(_overlap - value) < 0.001)
            {
                return;
            }

            _overlap = value;

            OnPropertyChanged();

            RebuildShotPlan();
        }
    }

    public double EffectiveStep =>
        _shotPlan?.StepLengthMm ??
        Math.Max(1, ShotSize - Overlap);

    public int TotalShots =>
        _shotPlan?.TotalShots ??
        CalculateTotalShots();

    public int CurrentShotNumber =>
        _shotPlan?.CurrentShotNumber ??
        _currentShotNumber;

    public double CurrentShotStart =>
        _shotPlan?.CurrentStartPositionMm ??
        0;

    public double CurrentShotEnd =>
        _shotPlan?.CurrentEndPositionMm ??
        0;

    public string ShotPosition =>
        $"Shot {CurrentShotNumber}  |  " +
        $"{CurrentShotStart:0} - {CurrentShotEnd:0} mm";

    public bool IsConnected =>
        ConnectionStatus == "Connected";

    public bool HasCapturedImage =>
        _capturedImage != null;

    public ImageSource? CapturedImageSource
    {
        get => _capturedImageSource;
        private set
        {
            _capturedImageSource = value;
            OnPropertyChanged();
        }
    }

    public bool HasShotPlan =>
        _shotPlan?.HasShots == true;

    public ShotPlanModel? ShotPlan =>
        _shotPlan;

    public AcquisitionViewModel()
    {
        UpdateCurrentJob();

        CurrentJobService.Instance.CurrentJobChanged +=
            (_, _) =>
            {
                UpdateCurrentJob();
            };

        ConnectCommand =
            new RelayCommand(
                _ => ConnectDetector());

        CaptureCommand =
            new RelayCommand(
                _ => CaptureImage());

        SaveCommand =
            new RelayCommand(
                _ => SaveImage());

        RetakeCommand =
            new RelayCommand(
                _ => RetakeImage());

        WindowLevelCommand =
            new RelayCommand(
                _ => ApplyWindowLevel());

        ZoomCommand =
            new RelayCommand(
                _ => ApplyZoom());

        RebuildShotPlan();
    }

    private void UpdateCurrentJob()
    {
        var job =
            CurrentJobService.Instance.CurrentJob;

        if (job != null)
        {
            CurrentJob =
                job.JobNumber;

            AcquisitionStatus =
                "Ready";
        }
        else
        {
            CurrentJob =
                "No Active Job";

            AcquisitionStatus =
                "Open a job before acquisition";
        }

        RebuildShotPlan();
    }

    private int CalculateTotalShots()
    {
        if (PipeLength <= 0 ||
            ShotSize <= 0)
        {
            return 0;
        }

        if (PipeLength <= ShotSize)
        {
            return 1;
        }

        double step =
            Math.Max(
                1,
                ShotSize - Overlap);

        return
            (int)Math.Ceiling(
                (PipeLength - ShotSize) /
                step) + 1;
    }

    private void RebuildShotPlan()
    {
        var job =
            CurrentJobService.Instance.CurrentJob;

        Guid jobId =
            job?.Id ?? Guid.Empty;

        if (PipeLength <= 0 ||
            ShotSize <= 0 ||
            string.IsNullOrWhiteSpace(PipeId))
        {
            _shotPlan = null;

            OnShotPlanChanged();

            return;
        }

        int previousShot =
            Math.Max(
                1,
                _currentShotNumber);

        _shotPlan =
            new ShotPlanModel
            {
                JobId = jobId,
                PipeId = PipeId,
                PipeLengthMm = PipeLength,
                ShotLengthMm = ShotSize,
                OverlapMm = Overlap,
                RulerEnabled = true,
                PipeIdOverlayEnabled = true,
                AcquisitionMode = "Manual",
                Direction = "LeftToRight",
                Status = "Ready"
            };

        int totalShots =
            CalculateTotalShots();

        var shots =
            new System.Collections.Generic.List<ShotPlanItemModel>();

        for (int shotNumber = 1;
             shotNumber <= totalShots;
             shotNumber++)
        {
            double start =
                (shotNumber - 1) *
                _shotPlan.StepLengthMm;

            double end =
                Math.Min(
                    start + ShotSize,
                    PipeLength);

            shots.Add(
                new ShotPlanItemModel
                {
                    ShotPlanId = _shotPlan.Id,
                    JobId = jobId,
                    PipeId = PipeId,
                    ShotNumber = shotNumber,
                    StartPositionMm = start,
                    EndPositionMm = end,
                    NominalShotLengthMm = ShotSize,
                    ActualCoverageMm = end - start,
                    OverlapMm = Overlap,
                    RulerStartMm = start,
                    RulerEndMm = end,
                    AcquisitionMode = "Manual",
                    Status = "Pending"
                });
        }

        _shotPlan.SetShots(shots);

        if (previousShot > 1 &&
            previousShot <= totalShots)
        {
            _shotPlan.MoveToShot(
                previousShot);
        }

        _currentShotNumber =
            _shotPlan.CurrentShotNumber;

        OnShotPlanChanged();
    }

    private void OnShotPlanChanged()
    {
        OnPropertyChanged(nameof(ShotPlan));
        OnPropertyChanged(nameof(HasShotPlan));
        OnPropertyChanged(nameof(TotalShots));
        OnPropertyChanged(nameof(CurrentShotNumber));
        OnPropertyChanged(nameof(CurrentShotStart));
        OnPropertyChanged(nameof(CurrentShotEnd));
        OnPropertyChanged(nameof(EffectiveStep));
        OnPropertyChanged(nameof(ShotPosition));
    }

    private void MoveToNextShot()
    {
        if (_shotPlan == null)
        {
            return;
        }

        bool moved =
            _shotPlan.MoveToNextPendingShot();

        if (!moved)
        {
            _currentShotNumber =
                _shotPlan.CurrentShotNumber;

            OnShotPlanChanged();

            AcquisitionStatus =
                $"All {_shotPlan.TotalShots} shots completed.";

            return;
        }

        _currentShotNumber =
            _shotPlan.CurrentShotNumber;

        OnShotPlanChanged();

        AcquisitionStatus =
            $"Ready for Shot {CurrentShotNumber} of {TotalShots}";
    }

    private void ConnectDetector()
    {
        DetectorStatus =
            "Connecting...";

        ConnectionStatus =
            "Connecting...";

        AcquisitionStatus =
            "Connecting to detector...";

        ConnectionStatus =
            "Connected";

        DetectorStatus =
            "Ready";

        AcquisitionStatus =
            "Detector connected";

        OnPropertyChanged(
            nameof(IsConnected));
    }

    private void CaptureImage()
    {
        var job =
            CurrentJobService.Instance.CurrentJob;

        if (job == null)
        {
            AcquisitionStatus =
                "No active job. Open or create a job first.";

            return;
        }

        if (string.IsNullOrWhiteSpace(PipeId))
        {
            AcquisitionStatus =
                "Enter Pipe ID before capture.";

            return;
        }

        if (!IsConnected)
        {
            AcquisitionStatus =
                "Detector is not connected.";

            return;
        }

        if (KV <= 0 ||
            MA <= 0 ||
            ExposureTime <= 0)
        {
            AcquisitionStatus =
                "Exposure values must be greater than zero.";

            return;
        }

        if (PipeLength <= 0 ||
            ShotSize <= 0)
        {
            AcquisitionStatus =
                "Enter valid pipe length and shot size.";

            return;
        }

        if (_shotPlan == null ||
            !_shotPlan.HasShots)
        {
            RebuildShotPlan();
        }

        if (_shotPlan == null ||
            !_shotPlan.HasCurrentShot)
        {
            AcquisitionStatus =
                "Shot plan is not available.";

            return;
        }

        FrameNumber++;

        string imageFolder =
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "CapturedImages");

        Directory.CreateDirectory(
            imageFolder);

        string fileName =
            $"IMG_{FrameNumber:0000}.png";

        string filePath =
            Path.Combine(
                imageFolder,
                fileName);

        try
        {
            ShotPlanItemModel shot =
                _shotPlan.CurrentShot!;

            CreateVirtualRadiographyImage(
                filePath,
                FrameNumber);

            AddPermanentShotOverlay(
                filePath,
                PipeId,
                shot);

            string shotRemarks =
                $"Pipe ID {PipeId}; " +
                $"Shot {shot.ShotNumber}/{TotalShots}; " +
                $"Position {shot.StartPositionMm:0}-{shot.EndPositionMm:0} mm; " +
                $"Ruler {shot.RulerStartMm:0}-{shot.RulerEndMm:0} mm; " +
                $"Pipe Length {PipeLength:0} mm; " +
                $"Shot Size {ShotSize:0} mm; " +
                $"Overlap {Overlap:0} mm";

            _capturedImage =
                new ImageRecordModel
                {
                    JobId = job.Id,
                    JobNumber = job.JobNumber,
                    PipeId = PipeId,
                    Operator = job.Operator,

                    FrameNumber = FrameNumber,

                    FileName = fileName,
                    FilePath = filePath,

                    DetectorName =
                        "Virtual Detector",

                    KV = KV,
                    MA = MA,
                    ExposureTime = ExposureTime,

                    ImageWidth = 1024,
                    ImageHeight = 768,
                    BitDepth = 8,

                    ShotNumber =
                        shot.ShotNumber,

                    TotalShots =
                        TotalShots,

                    PipeLength =
                        PipeLength,

                    ShotSize =
                        ShotSize,

                    Overlap =
                        Overlap,

                    ShotStartPosition =
                        shot.StartPositionMm,

                    ShotEndPosition =
                        shot.EndPositionMm,

                    Remarks =
                        shotRemarks,

                    CapturedOn =
                        DateTime.Now
                };

            CapturedImageSource =
                CreateImageSource(filePath);

            AcquisitionStatus =
                $"Pipe {PipeId} | " +
                $"Shot {shot.ShotNumber}/{TotalShots} captured " +
                $"({shot.StartPositionMm:0}-{shot.EndPositionMm:0} mm)";

            OnPropertyChanged(
                nameof(HasCapturedImage));

            ImageViewerService.Instance.OpenImage(
                _capturedImage);
        }
        catch (Exception ex)
        {
            FrameNumber--;

            _capturedImage = null;
            CapturedImageSource = null;

            AcquisitionStatus =
                $"Capture failed: {ex.Message}";

            OnPropertyChanged(
                nameof(HasCapturedImage));
        }
    }

    private static ImageSource? CreateImageSource(
        string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath) ||
            !File.Exists(filePath))
        {
            return null;
        }

        var bitmap =
            new BitmapImage();

        bitmap.BeginInit();

        bitmap.UriSource =
            new Uri(
                filePath,
                UriKind.Absolute);

        bitmap.CacheOption =
            BitmapCacheOption.OnLoad;

        bitmap.CreateOptions =
            BitmapCreateOptions.PreservePixelFormat;

        bitmap.EndInit();

        bitmap.Freeze();

        return bitmap;
    }

    private static void AddPermanentShotOverlay(
        string filePath,
        string pipeId,
        ShotPlanItemModel shot)
    {
        const int width = 1024;
        const int height = 768;

        BitmapFrame sourceFrame;

        using (FileStream input =
               new(
                   filePath,
                   FileMode.Open,
                   FileAccess.Read,
                   FileShare.Read))
        {
            var decoder =
                new PngBitmapDecoder(
                    input,
                    BitmapCreateOptions.PreservePixelFormat,
                    BitmapCacheOption.OnLoad);

            sourceFrame =
                decoder.Frames[0];
        }

        var visual =
            new DrawingVisual();

        using (DrawingContext dc =
               visual.RenderOpen())
        {
            dc.DrawImage(
                sourceFrame,
                new Rect(
                    0,
                    0,
                    width,
                    height));

            var overlayBackground =
                new SolidColorBrush(
                    Color.FromArgb(
                        190,
                        0,
                        0,
                        0));

            overlayBackground.Freeze();

            var whiteBrush =
                Brushes.White;

            var cyanBrush =
                Brushes.Cyan;

            var yellowBrush =
                Brushes.Yellow;

            var rulerPen =
                new Pen(
                    whiteBrush,
                    2);

            rulerPen.Freeze();

            var tickPen =
                new Pen(
                    whiteBrush,
                    1);

            tickPen.Freeze();

            var majorTickPen =
                new Pen(
                    yellowBrush,
                    2);

            majorTickPen.Freeze();

            dc.DrawRoundedRectangle(
                overlayBackground,
                null,
                new Rect(
                    20,
                    18,
                    430,
                    58),
                6,
                6);

            DrawText(
                dc,
                $"PIPE ID: {pipeId}",
                35,
                25,
                22,
                cyanBrush);

            DrawText(
                dc,
                $"SHOT {shot.ShotNumber} | " +
                $"{shot.StartPositionMm:0} - {shot.EndPositionMm:0} mm",
                35,
                51,
                15,
                whiteBrush);

            const double rulerLeft = 70;
            const double rulerRight = 954;
            const double rulerY = 690;

            dc.DrawRoundedRectangle(
                overlayBackground,
                null,
                new Rect(
                    35,
                    650,
                    954,
                    100),
                6,
                6);

            dc.DrawLine(
                rulerPen,
                new Point(
                    rulerLeft,
                    rulerY),
                new Point(
                    rulerRight,
                    rulerY));

            double rulerStart =
                shot.RulerStartMm;

            double rulerEnd =
                shot.RulerEndMm;

            double span =
                rulerEnd - rulerStart;

            if (span <= 0)
            {
                span = 1;
            }

            int firstTick =
                (int)Math.Ceiling(
                    rulerStart / 10.0) * 10;

            for (double position = firstTick;
                 position <= rulerEnd + 0.001;
                 position += 10)
            {
                double ratio =
                    (position - rulerStart) /
                    span;

                ratio =
                    Math.Clamp(
                        ratio,
                        0,
                        1);

                double x =
                    rulerLeft +
                    ((rulerRight - rulerLeft) *
                     ratio);

                bool isMajor =
                    Math.Abs(
                        position % 50) <
                    0.001;

                double tickHeight =
                    isMajor
                        ? 26
                        : 13;

                dc.DrawLine(
                    isMajor
                        ? majorTickPen
                        : tickPen,
                    new Point(
                        x,
                        rulerY),
                    new Point(
                        x,
                        rulerY - tickHeight));

                if (isMajor)
                {
                    DrawTextCentered(
                        dc,
                        $"{position:0}",
                        x,
                        rulerY + 8,
                        13,
                        yellowBrush);
                }
            }

            DrawText(
                dc,
                "PIPE POSITION (mm)",
                rulerLeft,
                658,
                13,
                whiteBrush);
        }

        var rendered =
            new RenderTargetBitmap(
                width,
                height,
                96,
                96,
                PixelFormats.Pbgra32);

        rendered.Render(
            visual);

        rendered.Freeze();

        var encoder =
            new PngBitmapEncoder();

        encoder.Frames.Add(
            BitmapFrame.Create(
                rendered));

        using FileStream output =
            new(
                filePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None);

        encoder.Save(output);
    }

    private static void DrawText(
        DrawingContext dc,
        string text,
        double x,
        double y,
        double fontSize,
        Brush brush)
    {
        var formattedText =
            new FormattedText(
                text,
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new Typeface(
                    "Segoe UI"),
                fontSize,
                brush,
                1.0);

        dc.DrawText(
            formattedText,
            new Point(
                x,
                y));
    }

    private static void DrawTextCentered(
        DrawingContext dc,
        string text,
        double centerX,
        double y,
        double fontSize,
        Brush brush)
    {
        var formattedText =
            new FormattedText(
                text,
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new Typeface(
                    "Segoe UI"),
                fontSize,
                brush,
                1.0);

        dc.DrawText(
            formattedText,
            new Point(
                centerX -
                (formattedText.Width / 2),
                y));
    }

    private static void CreateVirtualRadiographyImage(
        string filePath,
        int frameNumber)
    {
        const int width = 1024;
        const int height = 768;

        var bitmap =
            new WriteableBitmap(
                width,
                height,
                96,
                96,
                PixelFormats.Bgra32,
                null);

        int stride =
            width * 4;

        byte[] pixels =
            new byte[
                stride *
                height];

        double centerX =
            width / 2.0;

        double centerY =
            height / 2.0;

        double pipeRadius =
            Math.Min(
                width,
                height) *
            0.31;

        double pipeInnerRadius =
            pipeRadius * 0.72;

        for (int y = 0;
             y < height;
             y++)
        {
            for (int x = 0;
                 x < width;
                 x++)
            {
                double dx =
                    x - centerX;

                double dy =
                    y - centerY;

                double distance =
                    Math.Sqrt(
                        (dx * dx) +
                        (dy * dy));

                byte intensity;

                if (distance <= pipeRadius &&
                    distance >= pipeInnerRadius)
                {
                    intensity = 150;
                }
                else if (distance < pipeInnerRadius)
                {
                    intensity = 52;
                }
                else
                {
                    double gradient =
                        18 +
                        (170.0 *
                         (double)y /
                         height);

                    intensity =
                        (byte)Math.Clamp(
                            gradient,
                            10,
                            220);
                }

                double weldY =
                    centerY +
                    Math.Sin(
                        x * 0.025) *
                    18;

                if (Math.Abs(
                        y - weldY) < 5 &&
                    distance < pipeRadius &&
                    distance > pipeInnerRadius)
                {
                    intensity = 235;
                }

                double defect1 =
                    Math.Sqrt(
                        Math.Pow(
                            x - 430,
                            2) +
                        Math.Pow(
                            y - 355,
                            2));

                double defect2 =
                    Math.Sqrt(
                        Math.Pow(
                            x - 610,
                            2) +
                        Math.Pow(
                            y - 405,
                            2));

                if (defect1 < 13 ||
                    defect2 < 9)
                {
                    intensity = 245;
                }

                int variation =
                    frameNumber % 12;

                intensity =
                    (byte)Math.Clamp(
                        intensity + variation,
                        0,
                        255);

                int index =
                    (y * stride) +
                    (x * 4);

                pixels[index] =
                    intensity;

                pixels[index + 1] =
                    intensity;

                pixels[index + 2] =
                    intensity;

                pixels[index + 3] =
                    255;
            }
        }

        bitmap.WritePixels(
            new Int32Rect(
                0,
                0,
                width,
                height),
            pixels,
            stride,
            0);

        bitmap.Freeze();

        var encoder =
            new PngBitmapEncoder();

        encoder.Frames.Add(
            BitmapFrame.Create(
                bitmap));

        using FileStream stream =
            new(
                filePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None);

        encoder.Save(stream);
    }

    private void SaveImage()
    {
        if (_capturedImage == null)
        {
            AcquisitionStatus =
                "No captured image available to save.";

            return;
        }

        try
        {
            _imageService.Save(
                _capturedImage);

            if (_shotPlan != null)
            {
                _shotPlan.MarkCurrentShotCaptured(
                    _capturedImage.Id,
                    _capturedImage.FileName);
            }

            string savedStatus =
                $"Image {_capturedImage.FileName} saved";

            _capturedImage = null;
            CapturedImageSource = null;

            OnPropertyChanged(
                nameof(HasCapturedImage));

            if (_shotPlan != null &&
                !_shotPlan.IsCompleted)
            {
                MoveToNextShot();

                AcquisitionStatus =
                    $"{savedStatus}. Ready for {ShotPosition}";
            }
            else
            {
                OnShotPlanChanged();

                AcquisitionStatus =
                    $"{savedStatus}. " +
                    $"All {TotalShots} shots completed.";
            }
        }
        catch (Exception ex)
        {
            AcquisitionStatus =
                $"Image save failed: {ex.Message}";
        }
    }

    private void RetakeImage()
    {
        if (_capturedImage == null)
        {
            AcquisitionStatus =
                "No captured image to retake.";

            return;
        }

        _capturedImage = null;
        CapturedImageSource = null;

        if (FrameNumber > 0)
        {
            FrameNumber--;
        }

        AcquisitionStatus =
            $"Ready for retake: {ShotPosition}";

        OnPropertyChanged(
            nameof(HasCapturedImage));
    }

    private void ApplyWindowLevel()
    {
        if (_capturedImage == null)
        {
            AcquisitionStatus =
                "Capture an image before adjusting Window / Level.";

            return;
        }

        AcquisitionStatus =
            "Window / Level adjustment ready";
    }

    private void ApplyZoom()
    {
        if (_capturedImage == null)
        {
            AcquisitionStatus =
                "Capture an image before using Zoom.";

            return;
        }

        AcquisitionStatus =
            "Zoom adjustment ready";
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