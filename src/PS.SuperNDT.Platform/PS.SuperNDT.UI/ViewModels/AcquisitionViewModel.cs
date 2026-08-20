using System;
using System.Collections.Generic;
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
            new List<ShotPlanItemModel>();

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

            var topBackground =
                new SolidColorBrush(
                    Color.FromArgb(
                        205,
                        0,
                        0,
                        0));

            topBackground.Freeze();

            var rulerBackground =
                new SolidColorBrush(
                    Color.FromArgb(
                        220,
                        0,
                        0,
                        0));

            rulerBackground.Freeze();

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

            /*
             * TOP INFORMATION
             */

            dc.DrawRoundedRectangle(
                topBackground,
                null,
                new Rect(
                    20,
                    18,
                    500,
                    68),
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
                $"{shot.StartPositionMm:0} - " +
                $"{shot.EndPositionMm:0} mm",
                35,
                53,
                16,
                whiteBrush);

            /*
             * FULL FRAME RULER
             */

            const double rulerLeft = 32;
            const double rulerRight = 992;
            const double rulerY = 700;

            dc.DrawRoundedRectangle(
                rulerBackground,
                null,
                new Rect(
                    12,
                    638,
                    1000,
                    116),
                5,
                5);

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
                        ? 28
                        : 14;

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
                652,
                13,
                whiteBrush);

            DrawText(
                dc,
                $"{rulerStart:0} mm",
                rulerLeft,
                716,
                13,
                whiteBrush);

            DrawTextCentered(
                dc,
                $"{rulerEnd:0} mm",
                rulerRight,
                716,
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

        const int materialTop = 105;
        const int materialBottom = 625;

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

        /*
         * BACKGROUND
         */

        for (int y = 0;
             y < height;
             y++)
        {
            for (int x = 0;
                 x < width;
                 x++)
            {
                double gradient =
                    20 +
                    (105.0 *
                     y /
                     height);

                double texture =
                    Math.Sin(
                        x * 0.017) *
                    2.5;

                byte intensity =
                    (byte)Math.Clamp(
                        gradient +
                        texture,
                        8,
                        170);

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

        /*
         * PIPE MATERIAL
         *
         * Long rectangular DR field representing
         * the pipe wall in the current shot.
         */

        for (int y = materialTop;
             y < materialBottom;
             y++)
        {
            for (int x = 25;
                 x < width - 25;
                 x++)
            {
                double baseIntensity =
                    108 +
                    (38.0 *
                     (double)y /
                     (materialBottom -
                      materialTop));

                double texture =
                    Math.Sin(
                        x * 0.029) *
                    3.0;

                double fineTexture =
                    Math.Sin(
                        y * 0.047) *
                    2.0;

                byte intensity =
                    (byte)Math.Clamp(
                        baseIntensity +
                        texture +
                        fineTexture,
                        65,
                        185);

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

        /*
         * TOP AND BOTTOM PIPE BOUNDARIES
         */

        for (int x = 25;
             x < width - 25;
             x++)
        {
            for (int thickness = 0;
                 thickness < 5;
                 thickness++)
            {
                int topY =
                    materialTop +
                    thickness;

                int bottomY =
                    materialBottom -
                    thickness -
                    1;

                int topIndex =
                    (topY * stride) +
                    (x * 4);

                int bottomIndex =
                    (bottomY * stride) +
                    (x * 4);

                byte edge =
                    (byte)
                    Math.Clamp(
                        170 -
                        (thickness * 20),
                        80,
                        200);

                pixels[topIndex] =
                    edge;

                pixels[topIndex + 1] =
                    edge;

                pixels[topIndex + 2] =
                    edge;

                pixels[topIndex + 3] =
                    255;

                pixels[bottomIndex] =
                    edge;

                pixels[bottomIndex + 1] =
                    edge;

                pixels[bottomIndex + 2] =
                    edge;

                pixels[bottomIndex + 3] =
                    255;
            }
        }

        /*
         * LONGITUDINAL SEAM WELD
         *
         * Runs along the pipe length.
         * The pipe length is represented horizontally.
         */

        const int seamCenterY = 355;

        for (int x = 30;
             x < width - 30;
             x++)
        {
            double wave =
                Math.Sin(
                    x * 0.026) *
                3.0;

            int centerY =
                seamCenterY +
                (int)wave;

            for (int dy = -9;
                 dy <= 9;
                 dy++)
            {
                int y =
                    centerY + dy;

                if (y < materialTop ||
                    y >= materialBottom)
                {
                    continue;
                }

                double distance =
                    Math.Abs(dy);

                byte weldIntensity =
                    (byte)Math.Clamp(
                        218 -
                        (distance * 12),
                        85,
                        240);

                int index =
                    (y * stride) +
                    (x * 4);

                pixels[index] =
                    weldIntensity;

                pixels[index + 1] =
                    weldIntensity;

                pixels[index + 2] =
                    weldIntensity;

                pixels[index + 3] =
                    255;
            }
        }

        /*
         * SEAM ROOT / FUSION LINE
         */

        for (int x = 30;
             x < width - 30;
             x++)
        {
            double wave =
                Math.Sin(
                    x * 0.026) *
                3.0;

            int y =
                seamCenterY +
                (int)wave;

            int index =
                (y * stride) +
                (x * 4);

            pixels[index] =
                250;

            pixels[index + 1] =
                250;

            pixels[index + 2] =
                250;

            pixels[index + 3] =
                255;
        }

        /*
         * CIRCUMFERENTIAL / HORIZONTAL WELD
         *
         * Crosses the pipe wall vertically in this
         * representation.
         */

        const int circumferentialCenterX = 735;

        for (int y = materialTop;
             y < materialBottom;
             y++)
        {
            double wave =
                Math.Sin(
                    y * 0.035) *
                4.0;

            int centerX =
                circumferentialCenterX +
                (int)wave;

            for (int dx = -10;
                 dx <= 10;
                 dx++)
            {
                int x =
                    centerX + dx;

                if (x < 25 ||
                    x >= width - 25)
                {
                    continue;
                }

                double distance =
                    Math.Abs(dx);

                byte weldIntensity =
                    (byte)Math.Clamp(
                        220 -
                        (distance * 11),
                        90,
                        245);

                int index =
                    (y * stride) +
                    (x * 4);

                pixels[index] =
                    weldIntensity;

                pixels[index + 1] =
                    weldIntensity;

                pixels[index + 2] =
                    weldIntensity;

                pixels[index + 3] =
                    255;
            }
        }

        /*
         * CIRCUMFERENTIAL WELD ROOT
         */

        for (int y = materialTop;
             y < materialBottom;
             y++)
        {
            double wave =
                Math.Sin(
                    y * 0.035) *
                4.0;

            int x =
                circumferentialCenterX +
                (int)wave;

            int index =
                (y * stride) +
                (x * 4);

            pixels[index] =
                250;

            pixels[index + 1] =
                250;

            pixels[index + 2] =
                250;

            pixels[index + 3] =
                255;
        }

        /*
         * DEFECT 1
         *
         * Small rounded indication close to
         * longitudinal seam.
         */

        DrawDefect(
            pixels,
            stride,
            430,
            355,
            14,
            248);

        /*
         * DEFECT 2
         *
         * Smaller indication close to
         * circumferential weld.
         */

        DrawDefect(
            pixels,
            stride,
            735,
            475,
            10,
            252);

        /*
         * DEFECT 3
         *
         * Small linear indication inside pipe wall.
         */

        DrawLinearDefect(
            pixels,
            stride,
            545,
            465,
            580,
            485,
            6,
            242);

        /*
         * FRAME VARIATION
         */

        int variation =
            frameNumber % 8;

        if (variation != 0)
        {
            for (int y = materialTop;
                 y < materialBottom;
                 y++)
            {
                for (int x = 25;
                     x < width - 25;
                     x++)
                {
                    int index =
                        (y * stride) +
                        (x * 4);

                    int value =
                        pixels[index] +
                        variation;

                    byte adjusted =
                        (byte)Math.Clamp(
                            value,
                            0,
                            255);

                    pixels[index] =
                        adjusted;

                    pixels[index + 1] =
                        adjusted;

                    pixels[index + 2] =
                        adjusted;
                }
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

    private static void DrawDefect(
        byte[] pixels,
        int stride,
        double centerX,
        double centerY,
        double radius,
        byte intensity)
    {
        int minX =
            Math.Max(
                0,
                (int)(centerX - radius - 1));

        int maxX =
            Math.Min(
                1023,
                (int)(centerX + radius + 1));

        int minY =
            Math.Max(
                0,
                (int)(centerY - radius - 1));

        int maxY =
            Math.Min(
                767,
                (int)(centerY + radius + 1));

        for (int y = minY;
             y <= maxY;
             y++)
        {
            for (int x = minX;
                 x <= maxX;
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

                if (distance > radius)
                {
                    continue;
                }

                int index =
                    (y * stride) +
                    (x * 4);

                byte value =
                    distance <
                    radius * 0.55
                        ? intensity
                        : (byte)Math.Max(
                            150,
                            intensity - 45);

                pixels[index] =
                    value;

                pixels[index + 1] =
                    value;

                pixels[index + 2] =
                    value;

                pixels[index + 3] =
                    255;
            }
        }
    }

    private static void DrawLinearDefect(
        byte[] pixels,
        int stride,
        double startX,
        double startY,
        double endX,
        double endY,
        double thickness,
        byte intensity)
    {
        double length =
            Math.Sqrt(
                Math.Pow(
                    endX - startX,
                    2) +
                Math.Pow(
                    endY - startY,
                    2));

        if (length <= 0)
        {
            return;
        }

        int steps =
            Math.Max(
                1,
                (int)length);

        for (int i = 0;
             i <= steps;
             i++)
        {
            double ratio =
                (double)i /
                steps;

            double x =
                startX +
                ((endX - startX) *
                 ratio);

            double y =
                startY +
                ((endY - startY) *
                 ratio);

            DrawDefect(
                pixels,
                stride,
                x,
                y,
                thickness,
                intensity);
        }
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