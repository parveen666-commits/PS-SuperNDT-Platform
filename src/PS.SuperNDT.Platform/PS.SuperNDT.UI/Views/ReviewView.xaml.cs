using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class ReviewView : UserControl
{
    private const string PersistedDefectTagPrefix =
        "PERSISTED_DEFECT:";

    private const string RulerTickTag =
        "DYNAMIC_RULER_TICK";

    private const double MinimumZoom = 0.25;
    private const double MaximumZoom = 5.0;
    private const double ZoomStep = 0.25;
    private const double FitZoom = 1.0;

    private bool _isPanning;
    private Point _lastMousePosition;

    private TranslateTransform? _panTransform;
    private ScaleTransform? _scaleTransform;

    private bool _isDrawingDefect;
    private Point _defectStartPoint;
    private Point _defectCurrentPoint;

    private bool _updatingScrollMode;
    private bool _isFittingFrame;
    private bool _rulerRefreshPending;

    private Guid? _selectedDefectId;

    public ReviewView()
    {
        InitializeComponent();

        Focusable = true;
        IsTabStop = false;

        DataContext = new ReviewViewModel();

        Loaded += ReviewView_Loaded;
        Unloaded += ReviewView_Unloaded;

        PreviewKeyDown += ReviewView_PreviewKeyDown;
    }

    // ============================================================
    // LOADED
    // ============================================================

    private void ReviewView_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        Loaded -= ReviewView_Loaded;

        SetupPanTransform();

        ImagePanCanvas.MouseDown += ImagePanCanvas_MouseDown;
        ImagePanCanvas.MouseMove += ImagePanCanvas_MouseMove;
        ImagePanCanvas.MouseUp += ImagePanCanvas_MouseUp;
        ImagePanCanvas.MouseLeave += ImagePanCanvas_MouseLeave;

        ImageViewport.MouseDown += ImageViewport_MouseDown;
        ImageViewport.MouseMove += ImageViewport_MouseMove;
        ImageViewport.MouseUp += ImageViewport_MouseUp;
        ImageViewport.MouseLeave += ImageViewport_MouseLeave;

        ImageViewport.PreviewMouseWheel +=
            ImageViewport_MouseWheel;

        ImageViewport.SizeChanged +=
            ImageViewport_SizeChanged;

        if (DataContext is INotifyPropertyChanged notifyObject)
        {
            notifyObject.PropertyChanged +=
                ViewModel_PropertyChanged;
        }

        Dispatcher.BeginInvoke(
            new Action(() =>
            {
                Focus();

                FitImageToFrame();
                ApplyZoomVisual();
                UpdateScrollMode();
                SyncDefectOverlay();
                LoadSavedDefects();
                ScheduleRulerRefresh();
            }),
            DispatcherPriority.Loaded);
    }

    // ============================================================
    // UNLOADED
    // ============================================================

    private void ReviewView_Unloaded(
        object sender,
        RoutedEventArgs e)
    {
        if (DataContext is INotifyPropertyChanged notifyObject)
        {
            notifyObject.PropertyChanged -=
                ViewModel_PropertyChanged;
        }

        ImagePanCanvas.MouseDown -= ImagePanCanvas_MouseDown;
        ImagePanCanvas.MouseMove -= ImagePanCanvas_MouseMove;
        ImagePanCanvas.MouseUp -= ImagePanCanvas_MouseUp;
        ImagePanCanvas.MouseLeave -= ImagePanCanvas_MouseLeave;

        ImageViewport.MouseDown -= ImageViewport_MouseDown;
        ImageViewport.MouseMove -= ImageViewport_MouseMove;
        ImageViewport.MouseUp -= ImageViewport_MouseUp;
        ImageViewport.MouseLeave -= ImageViewport_MouseLeave;

        ImageViewport.PreviewMouseWheel -=
            ImageViewport_MouseWheel;

        ImageViewport.SizeChanged -=
            ImageViewport_SizeChanged;

        ClearPersistedDefectRectangles();
        ClearDynamicRulers();

        _selectedDefectId = null;
        _isPanning = false;
        _isDrawingDefect = false;
        _rulerRefreshPending = false;

        Mouse.Capture(null);
    }

    // ============================================================
    // VIEW MODEL
    // ============================================================

    private void ViewModel_PropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (string.Equals(
                e.PropertyName,
                nameof(ReviewViewModel.ZoomLevel),
                StringComparison.Ordinal))
        {
            Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    ApplyZoomVisual();

                    double zoom = GetZoom();

                    if (Math.Abs(
                            zoom - FitZoom) < 0.001)
                    {
                        FitImageToFrame();
                    }
                    else
                    {
                        UpdateScrollMode();
                        ClampPan();
                    }

                    SyncDefectOverlay();
                    RefreshSavedDefects();
                    ScheduleRulerRefresh();
                }),
                DispatcherPriority.Render);

            return;
        }

        if (string.Equals(
                e.PropertyName,
                nameof(ReviewViewModel.SelectedImage),
                StringComparison.Ordinal) ||
            string.Equals(
                e.PropertyName,
                nameof(ReviewViewModel.DisplayImage),
                StringComparison.Ordinal))
        {
            _selectedDefectId = null;

            Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    ApplyZoomVisual();

                    double zoom = GetZoom();

                    if (Math.Abs(
                            zoom - FitZoom) < 0.001)
                    {
                        FitImageToFrame();
                    }
                    else
                    {
                        UpdateScrollMode();
                        ClampPan();
                    }

                    SyncDefectOverlay();
                    RefreshSavedDefects();
                    ScheduleRulerRefresh();
                }),
                DispatcherPriority.Render);
        }
    }

    // ============================================================
    // FIT IMAGE
    // ============================================================

    private void FitImageToFrame()
    {
        if (!IsLoaded ||
            _isFittingFrame)
        {
            return;
        }

        if (ImageViewport.ActualWidth <= 20 ||
            ImageViewport.ActualHeight <= 20)
        {
            return;
        }

        _isFittingFrame = true;

        try
        {
            _isPanning = false;

            Mouse.Capture(null);

            ImageViewport.Cursor = Cursors.Arrow;
            ImagePanCanvas.Cursor = Cursors.Arrow;

            ImageViewport.HorizontalScrollBarVisibility =
                ScrollBarVisibility.Hidden;

            ImageViewport.VerticalScrollBarVisibility =
                ScrollBarVisibility.Hidden;

            ImageViewport.ScrollToHorizontalOffset(0);
            ImageViewport.ScrollToVerticalOffset(0);

            if (_panTransform != null)
            {
                _panTransform.X = 0;
                _panTransform.Y = 0;
            }

            ImagePanCanvas.Width =
                Math.Max(
                    1,
                    ImageViewport.ActualWidth - 4);

            ImagePanCanvas.Height =
                Math.Max(
                    1,
                    ImageViewport.ActualHeight - 4);

            if (_scaleTransform != null)
            {
                _scaleTransform.ScaleX = FitZoom;
                _scaleTransform.ScaleY = FitZoom;
            }
        }
        finally
        {
            _isFittingFrame = false;
        }

        SyncDefectOverlay();
        RefreshSavedDefects();
        ScheduleRulerRefresh();
    }

    // ============================================================
    // ZOOM
    // ============================================================

    private double GetZoom()
    {
        if (DataContext is ReviewViewModel viewModel)
        {
            return Math.Clamp(
                viewModel.ZoomLevel,
                MinimumZoom,
                MaximumZoom);
        }

        return FitZoom;
    }

    private void ApplyZoomVisual()
    {
        if (!IsLoaded)
        {
            return;
        }

        if (_scaleTransform == null)
        {
            SetupPanTransform();
        }

        if (_scaleTransform == null)
        {
            return;
        }

        double zoom =
            Math.Clamp(
                GetZoom(),
                MinimumZoom,
                MaximumZoom);

        _scaleTransform.ScaleX = zoom;
        _scaleTransform.ScaleY = zoom;

        ScheduleRulerRefresh();
    }

    private static void SetZoomFromView(
        ReviewViewModel viewModel,
        double zoom)
    {
        zoom =
            Math.Clamp(
                zoom,
                MinimumZoom,
                MaximumZoom);

        if (Math.Abs(
                viewModel.ZoomLevel - zoom) < 0.001)
        {
            return;
        }

        if (zoom >
            viewModel.ZoomLevel)
        {
            while (viewModel.ZoomLevel <
                   zoom - 0.001)
            {
                viewModel.ZoomInCommand.Execute(null);
            }
        }
        else
        {
            while (viewModel.ZoomLevel >
                   zoom + 0.001)
            {
                viewModel.ZoomOutCommand.Execute(null);
            }
        }
    }

    // ============================================================
    // TRANSFORM
    // ============================================================

    private void SetupPanTransform()
    {
        if (ImagePanCanvas.RenderTransform
            is TransformGroup existingGroup)
        {
            foreach (
                Transform transform
                in existingGroup.Children)
            {
                if (transform is ScaleTransform scale)
                {
                    _scaleTransform = scale;
                }

                if (transform is TranslateTransform translate)
                {
                    _panTransform = translate;
                }
            }

            if (_scaleTransform == null)
            {
                _scaleTransform =
                    new ScaleTransform(
                        FitZoom,
                        FitZoom);

                existingGroup.Children.Insert(
                    0,
                    _scaleTransform);
            }

            if (_panTransform == null)
            {
                _panTransform =
                    new TranslateTransform();

                existingGroup.Children.Add(
                    _panTransform);
            }

            return;
        }

        TransformGroup transformGroup =
            new TransformGroup();

        _scaleTransform =
            new ScaleTransform(
                FitZoom,
                FitZoom);

        _panTransform =
            new TranslateTransform();

        transformGroup.Children.Add(
            _scaleTransform);

        transformGroup.Children.Add(
            _panTransform);

        ImagePanCanvas.RenderTransform =
            transformGroup;
    }

    // ============================================================
    // SCROLL
    // ============================================================

    private void UpdateScrollMode()
    {
        if (!IsLoaded ||
            _updatingScrollMode)
        {
            return;
        }

        _updatingScrollMode = true;

        try
        {
            double zoom = GetZoom();

            if (zoom <= FitZoom + 0.001)
            {
                ImageViewport.HorizontalScrollBarVisibility =
                    ScrollBarVisibility.Hidden;

                ImageViewport.VerticalScrollBarVisibility =
                    ScrollBarVisibility.Hidden;

                ImageViewport.ScrollToHorizontalOffset(0);
                ImageViewport.ScrollToVerticalOffset(0);

                _isPanning = false;

                Mouse.Capture(null);

                ImageViewport.Cursor = Cursors.Arrow;
                ImagePanCanvas.Cursor = Cursors.Arrow;
            }
            else
            {
                ImageViewport.HorizontalScrollBarVisibility =
                    ScrollBarVisibility.Auto;

                ImageViewport.VerticalScrollBarVisibility =
                    ScrollBarVisibility.Auto;
            }
        }
        finally
        {
            _updatingScrollMode = false;
        }

        ScheduleRulerRefresh();
    }

    // ============================================================
    // RULER
    // ============================================================

    private void ScheduleRulerRefresh()
    {
        if (!IsLoaded ||
            RulerCanvas == null ||
            TopRulerCanvas == null)
        {
            return;
        }

        if (_rulerRefreshPending)
        {
            return;
        }

        _rulerRefreshPending = true;

        Dispatcher.BeginInvoke(
            new Action(() =>
            {
                _rulerRefreshPending = false;

                UpdateRulers();

                Dispatcher.BeginInvoke(
                    new Action(() =>
                    {
                        if (IsLoaded)
                        {
                            UpdateRulers();
                        }
                    }),
                    DispatcherPriority.ContextIdle);
            }),
            DispatcherPriority.Render);
    }

    private void UpdateRulers()
    {
        if (!IsLoaded ||
            TopRulerCanvas == null ||
            RulerCanvas == null ||
            ShotFrame == null)
        {
            return;
        }

        ClearDynamicRulers();

        if (DataContext is not ReviewViewModel viewModel ||
            viewModel.SelectedImage == null)
        {
            return;
        }

        double start =
            viewModel.SelectedImage.ShotStartPosition;

        double end =
            viewModel.SelectedImage.ShotEndPosition;

        double totalLength =
            end - start;

        if (totalLength <= 0)
        {
            return;
        }

        double rulerWidth =
            ShotFrame.ActualWidth;

        if (rulerWidth <= 1)
        {
            return;
        }

        RulerCanvas.Width =
            rulerWidth;

        TopRulerCanvas.Width =
            rulerWidth;

        double current =
            Math.Ceiling(
                start / 10.0) * 10.0;

        while (current <=
               end + 0.001)
        {
            double ratio =
                (current - start) /
                totalLength;

            double x =
                ratio * rulerWidth;

            if (x >= -20 &&
                x <= rulerWidth + 20)
            {
                bool major =
                    Math.Abs(
                        current % 50.0) < 0.001;

                DrawBottomRulerTick(
                    x,
                    major,
                    current);

                DrawTopRulerTick(
                    x,
                    major,
                    current);
            }

            current += 10.0;
        }
    }

    private void DrawBottomRulerTick(
        double x,
        bool major,
        double value)
    {
        Line tick =
            new Line
            {
                X1 = x,
                X2 = x,
                Y1 = 0,
                Y2 = major ? 18 : 9,

                Stroke =
                    new SolidColorBrush(
                        Color.FromRgb(
                            216,
                            222,
                            231)),

                StrokeThickness =
                    major ? 1.4 : 1,

                IsHitTestVisible = false,
                Tag = RulerTickTag
            };

        RulerCanvas.Children.Add(tick);

        if (!major)
        {
            return;
        }

        TextBlock label =
            new TextBlock
            {
                Text = $"{value:0}",

                Foreground =
                    new SolidColorBrush(
                        Color.FromRgb(
                            227,
                            232,
                            238)),

                FontSize = 9,
                IsHitTestVisible = false,
                Tag = RulerTickTag
            };

        Canvas.SetLeft(
            label,
            x + 2);

        Canvas.SetTop(
            label,
            18);

        RulerCanvas.Children.Add(label);
    }

    private void DrawTopRulerTick(
        double x,
        bool major,
        double value)
    {
        Line tick =
            new Line
            {
                X1 = x,
                X2 = x,
                Y1 = 29,
                Y2 = major ? 11 : 20,

                Stroke =
                    new SolidColorBrush(
                        Color.FromRgb(
                            216,
                            222,
                            231)),

                StrokeThickness =
                    major ? 1.4 : 1,

                IsHitTestVisible = false,
                Tag = RulerTickTag
            };

        TopRulerCanvas.Children.Add(tick);

        if (!major)
        {
            return;
        }

        TextBlock label =
            new TextBlock
            {
                Text = $"{value:0}",

                Foreground =
                    new SolidColorBrush(
                        Color.FromRgb(
                            227,
                            232,
                            238)),

                FontSize = 9,
                IsHitTestVisible = false,
                Tag = RulerTickTag
            };

        Canvas.SetLeft(
            label,
            x + 2);

        Canvas.SetTop(
            label,
            1);

        TopRulerCanvas.Children.Add(label);
    }

    private void ClearDynamicRulers()
    {
        RemoveDynamicRulerChildren(
            RulerCanvas);

        RemoveDynamicRulerChildren(
            TopRulerCanvas);
    }

    private static void RemoveDynamicRulerChildren(
        Canvas canvas)
    {
        for (
            int i =
                canvas.Children.Count - 1;
            i >= 0;
            i--)
        {
            if (canvas.Children[i]
                is FrameworkElement element &&
                string.Equals(
                    element.Tag as string,
                    RulerTickTag,
                    StringComparison.Ordinal))
            {
                canvas.Children.RemoveAt(i);
            }
        }
    }

    // ============================================================
    // IMAGE GEOMETRY
    // ============================================================

    private bool TryGetImageGeometry(
        out double imageWidth,
        out double imageHeight,
        out double offsetX,
        out double offsetY)
    {
        imageWidth = 0;
        imageHeight = 0;
        offsetX = 0;
        offsetY = 0;

        double frameWidth =
            ShotFrame.ActualWidth;

        double frameHeight =
            ShotFrame.ActualHeight;

        if (frameWidth <= 1 ||
            frameHeight <= 1)
        {
            return false;
        }

        imageWidth =
            frameWidth;

        imageHeight =
            frameHeight;

        return true;
    }

    private void SyncDefectOverlay()
    {
        if (!IsLoaded ||
            DefectOverlayCanvas == null ||
            ShotFrame == null)
        {
            return;
        }

        double width =
            ShotFrame.ActualWidth;

        double height =
            ShotFrame.ActualHeight;

        if (width <= 1 ||
            height <= 1)
        {
            return;
        }

        DefectOverlayCanvas.Width =
            width;

        DefectOverlayCanvas.Height =
            height;

        Canvas.SetLeft(
            DefectOverlayCanvas,
            0);

        Canvas.SetTop(
            DefectOverlayCanvas,
            0);
    }

    // ============================================================
    // PIXEL CONVERSION
    // ============================================================

    private double GetImagePixelScaleX()
    {
        if (DataContext is not ReviewViewModel viewModel ||
            viewModel.DisplayImage is not BitmapSource bitmap ||
            bitmap.Width <= 0)
        {
            return 1.0;
        }

        return bitmap.PixelWidth /
               bitmap.Width;
    }

    private double GetImagePixelScaleY()
    {
        if (DataContext is not ReviewViewModel viewModel ||
            viewModel.DisplayImage is not BitmapSource bitmap ||
            bitmap.Height <= 0)
        {
            return 1.0;
        }

        return bitmap.PixelHeight /
               bitmap.Height;
    }

    private Point DisplayPointToImagePixels(
        Point displayPoint)
    {
        return new Point(
            displayPoint.X *
            GetImagePixelScaleX(),

            displayPoint.Y *
            GetImagePixelScaleY());
    }

    private Point ImagePixelsToDisplayPoint(
        Point imagePoint)
    {
        double scaleX =
            GetImagePixelScaleX();

        double scaleY =
            GetImagePixelScaleY();

        if (scaleX <= 0)
        {
            scaleX = 1;
        }

        if (scaleY <= 0)
        {
            scaleY = 1;
        }

        return new Point(
            imagePoint.X / scaleX,
            imagePoint.Y / scaleY);
    }

    private double DisplayWidthToImagePixels(
        double width)
    {
        return width *
               GetImagePixelScaleX();
    }

    private double DisplayHeightToImagePixels(
        double height)
    {
        return height *
               GetImagePixelScaleY();
    }

    private double ImagePixelWidthToDisplay(
        double width)
    {
        double scale =
            GetImagePixelScaleX();

        return scale <= 0
            ? width
            : width / scale;
    }

    private double ImagePixelHeightToDisplay(
        double height)
    {
        double scale =
            GetImagePixelScaleY();

        return scale <= 0
            ? height
            : height / scale;
    }

    private Point ImagePointToOverlay(
        Point imagePoint)
    {
        if (!TryGetImageGeometry(
                out double imageWidth,
                out double imageHeight,
                out _,
                out _))
        {
            return imagePoint;
        }

        Point displayPoint =
            ImagePixelsToDisplayPoint(
                imagePoint);

        return new Point(
            Math.Clamp(
                displayPoint.X,
                0,
                imageWidth),

            Math.Clamp(
                displayPoint.Y,
                0,
                imageHeight));
    }

    private Point OverlayPointToImage(
        Point overlayPoint)
    {
        if (!TryGetImageGeometry(
                out double imageWidth,
                out double imageHeight,
                out _,
                out _))
        {
            return overlayPoint;
        }

        Point clamped =
            new Point(
                Math.Clamp(
                    overlayPoint.X,
                    0,
                    imageWidth),

                Math.Clamp(
                    overlayPoint.Y,
                    0,
                    imageHeight));

        return DisplayPointToImagePixels(
            clamped);
    }

    private Point GetMouseImagePoint(
        MouseEventArgs e)
    {
        Point point =
            e.GetPosition(
                DefectOverlayCanvas);

        if (!TryGetImageGeometry(
                out double imageWidth,
                out double imageHeight,
                out _,
                out _))
        {
            return point;
        }

        return new Point(
            Math.Clamp(
                point.X,
                0,
                imageWidth),

            Math.Clamp(
                point.Y,
                0,
                imageHeight));
    }

    // ============================================================
    // KEYBOARD
    // ============================================================

    private void ReviewView_PreviewKeyDown(
        object sender,
        KeyEventArgs e)
    {
        if (e.Key == Key.Delete)
        {
            if (_selectedDefectId.HasValue)
            {
                DeleteSelectedDefect();
                e.Handled = true;
            }

            return;
        }

        if (DataContext is not ReviewViewModel viewModel)
        {
            return;
        }

        if (e.Key == Key.Add ||
            e.Key == Key.OemPlus)
        {
            viewModel.ZoomInCommand.Execute(null);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Subtract ||
            e.Key == Key.OemMinus)
        {
            viewModel.ZoomOutCommand.Execute(null);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.D0 ||
            e.Key == Key.NumPad0)
        {
            viewModel.ResetZoomCommand.Execute(null);

            Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    FitImageToFrame();
                    ApplyZoomVisual();
                    UpdateScrollMode();
                    SyncDefectOverlay();
                    RefreshSavedDefects();
                    ScheduleRulerRefresh();
                }),
                DispatcherPriority.Render);

            e.Handled = true;
        }
    }

    // ============================================================
    // MOUSE WHEEL ZOOM
    // ============================================================

    private void ImageViewport_MouseWheel(
        object sender,
        MouseWheelEventArgs e)
    {
        if (DataContext is not ReviewViewModel viewModel)
        {
            return;
        }

        double oldZoom =
            Math.Clamp(
                viewModel.ZoomLevel,
                MinimumZoom,
                MaximumZoom);

        double newZoom =
            e.Delta > 0
                ? Math.Min(
                    MaximumZoom,
                    oldZoom + ZoomStep)
                : Math.Max(
                    MinimumZoom,
                    oldZoom - ZoomStep);

        if (Math.Abs(
                oldZoom - newZoom) < 0.001)
        {
            e.Handled = true;
            return;
        }

        if (_panTransform == null ||
            _scaleTransform == null)
        {
            SetupPanTransform();
        }

        Point mouse =
            e.GetPosition(
                ImageViewport);

        double centerX =
            ImageViewport.ActualWidth / 2.0;

        double centerY =
            ImageViewport.ActualHeight / 2.0;

        double relativeX =
            mouse.X - centerX;

        double relativeY =
            mouse.Y - centerY;

        double oldPanX =
            _panTransform?.X ?? 0;

        double oldPanY =
            _panTransform?.Y ?? 0;

        double ratio =
            newZoom / oldZoom;

        if (_panTransform != null)
        {
            _panTransform.X =
                relativeX -
                (relativeX - oldPanX) *
                ratio;

            _panTransform.Y =
                relativeY -
                (relativeY - oldPanY) *
                ratio;
        }

        SetZoomFromView(
            viewModel,
            newZoom);

        Dispatcher.BeginInvoke(
            new Action(() =>
            {
                ApplyZoomVisual();

                if (Math.Abs(
                        newZoom - FitZoom) < 0.001)
                {
                    FitImageToFrame();
                }
                else
                {
                    UpdateScrollMode();
                    ClampPan();
                }

                SyncDefectOverlay();
                RefreshSavedDefects();
                ScheduleRulerRefresh();
            }),
            DispatcherPriority.Render);

        e.Handled = true;
    }

    // ============================================================
    // PAN
    // ============================================================

    private bool IsZoomed()
    {
        return GetZoom() >
               FitZoom + 0.001;
    }

    private void StartPan(
        MouseButtonEventArgs e)
    {
        if (!IsZoomed())
        {
            return;
        }

        if (_panTransform == null)
        {
            SetupPanTransform();
        }

        if (_panTransform == null)
        {
            return;
        }

        _isPanning = true;

        _lastMousePosition =
            e.GetPosition(
                ImageViewport);

        Mouse.Capture(
            ImageViewport,
            CaptureMode.SubTree);

        ImageViewport.Cursor =
            Cursors.Hand;

        ImagePanCanvas.Cursor =
            Cursors.Hand;

        e.Handled = true;
    }

    private void MovePan(
        MouseEventArgs e)
    {
        if (!_isPanning ||
            _panTransform == null)
        {
            return;
        }

        Point current =
            e.GetPosition(
                ImageViewport);

        _panTransform.X +=
            current.X -
            _lastMousePosition.X;

        _panTransform.Y +=
            current.Y -
            _lastMousePosition.Y;

        _lastMousePosition =
            current;

        ClampPan();

        e.Handled = true;
    }

    private void EndPan(
        MouseButtonEventArgs e)
    {
        _isPanning = false;

        Mouse.Capture(null);

        ImageViewport.Cursor =
            Cursors.Arrow;

        ImagePanCanvas.Cursor =
            Cursors.Arrow;

        e.Handled = true;
    }

    private void ImagePanCanvas_MouseDown(
        object sender,
        MouseButtonEventArgs e)
    {
        if (e.ChangedButton ==
            MouseButton.Left)
        {
            if (Keyboard.Modifiers.HasFlag(
                    ModifierKeys.Shift))
            {
                StartDefectDrawing(e);
            }
            else
            {
                StartPan(e);
            }

            return;
        }

        if (e.ChangedButton ==
            MouseButton.Middle)
        {
            StartPan(e);
        }
    }

    private void ImageViewport_MouseDown(
        object sender,
        MouseButtonEventArgs e)
    {
        if (e.ChangedButton ==
            MouseButton.Left)
        {
            if (Keyboard.Modifiers.HasFlag(
                    ModifierKeys.Shift))
            {
                StartDefectDrawing(e);
            }
            else
            {
                StartPan(e);
            }

            return;
        }

        if (e.ChangedButton ==
            MouseButton.Middle)
        {
            StartPan(e);
        }
    }

    private void ImagePanCanvas_MouseMove(
        object sender,
        MouseEventArgs e)
    {
        if (_isDrawingDefect)
        {
            UpdateDefectDrawing(e);
            return;
        }

        if (_isPanning)
        {
            MovePan(e);
        }
    }

    private void ImageViewport_MouseMove(
        object sender,
        MouseEventArgs e)
    {
        if (_isDrawingDefect)
        {
            UpdateDefectDrawing(e);
            return;
        }

        if (_isPanning)
        {
            MovePan(e);
        }
    }

    private void ImagePanCanvas_MouseUp(
        object sender,
        MouseButtonEventArgs e)
    {
        if (_isDrawingDefect)
        {
            FinishDefectDrawing(e);
            return;
        }

        EndPan(e);
    }

    private void ImageViewport_MouseUp(
        object sender,
        MouseButtonEventArgs e)
    {
        if (_isDrawingDefect)
        {
            FinishDefectDrawing(e);
            return;
        }

        EndPan(e);
    }

    private void ImagePanCanvas_MouseLeave(
        object sender,
        MouseEventArgs e)
    {
        if (_isPanning)
        {
            ImageViewport.Cursor =
                Cursors.Hand;
        }
    }

    private void ImageViewport_MouseLeave(
        object sender,
        MouseEventArgs e)
    {
        if (_isPanning)
        {
            ImageViewport.Cursor =
                Cursors.Hand;
        }
    }

    // ============================================================
    // PAN CLAMP
    // ============================================================

    private void ClampPan()
    {
        if (_panTransform == null)
        {
            return;
        }

        double zoom =
            GetZoom();

        if (zoom <= FitZoom + 0.001)
        {
            _panTransform.X = 0;
            _panTransform.Y = 0;
            return;
        }

        double canvasWidth =
            ImagePanCanvas.ActualWidth;

        double canvasHeight =
            ImagePanCanvas.ActualHeight;

        double viewportWidth =
            ImageViewport.ActualWidth;

        double viewportHeight =
            ImageViewport.ActualHeight;

        double scaledWidth =
            canvasWidth * zoom;

        double scaledHeight =
            canvasHeight * zoom;

        double maxPanX =
            Math.Max(
                0,
                (scaledWidth -
                 viewportWidth) / 2.0);

        double maxPanY =
            Math.Max(
                0,
                (scaledHeight -
                 viewportHeight) / 2.0);

        _panTransform.X =
            Math.Clamp(
                _panTransform.X,
                -maxPanX,
                maxPanX);

        _panTransform.Y =
            Math.Clamp(
                _panTransform.Y,
                -maxPanY,
                maxPanY);
    }

    // ============================================================
    // VIEWPORT SIZE
    // ============================================================

    private void ImageViewport_SizeChanged(
        object sender,
        SizeChangedEventArgs e)
    {
        double zoom =
            GetZoom();

        if (Math.Abs(
                zoom - FitZoom) < 0.001)
        {
            Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    FitImageToFrame();
                    SyncDefectOverlay();
                    RefreshSavedDefects();
                    ScheduleRulerRefresh();
                }),
                DispatcherPriority.Render);
        }
        else
        {
            ApplyZoomVisual();
            UpdateScrollMode();
            ClampPan();
            SyncDefectOverlay();
            RefreshSavedDefects();
            ScheduleRulerRefresh();
        }
    }

    // ============================================================
    // DEFECT DRAWING
    // ============================================================

    private void StartDefectDrawing(
        MouseButtonEventArgs e)
    {
        if (e.ChangedButton !=
            MouseButton.Left ||
            !Keyboard.Modifiers.HasFlag(
                ModifierKeys.Shift))
        {
            return;
        }

        SyncDefectOverlay();

        Point point =
            GetMouseImagePoint(e);

        _isDrawingDefect = true;

        _defectStartPoint =
            point;

        _defectCurrentPoint =
            point;

        PrepareDefectRectangle();

        Mouse.Capture(
            ImageViewport,
            CaptureMode.SubTree);

        ImageViewport.Cursor =
            Cursors.Cross;

        ImagePanCanvas.Cursor =
            Cursors.Cross;

        UpdateDefectRectangle();

        e.Handled = true;
    }

    private void UpdateDefectDrawing(
        MouseEventArgs e)
    {
        if (!_isDrawingDefect)
        {
            return;
        }

        _defectCurrentPoint =
            GetMouseImagePoint(e);

        UpdateDefectRectangle();

        e.Handled = true;
    }

    private Point ClampPointToOverlay(
        Point point)
    {
        return new Point(
            Math.Clamp(
                point.X,
                0,
                Math.Max(
                    0,
                    DefectOverlayCanvas.ActualWidth)),

            Math.Clamp(
                point.Y,
                0,
                Math.Max(
                    0,
                    DefectOverlayCanvas.ActualHeight)));
    }

    private void PrepareDefectRectangle()
    {
        DefectRectangle.Visibility =
            Visibility.Visible;

        DefectRectangle.Width = 0;
        DefectRectangle.Height = 0;
    }

    private void UpdateDefectRectangle()
    {
        Point start =
            ClampPointToOverlay(
                _defectStartPoint);

        Point current =
            ClampPointToOverlay(
                _defectCurrentPoint);

        double left =
            Math.Min(
                start.X,
                current.X);

        double top =
            Math.Min(
                start.Y,
                current.Y);

        double width =
            Math.Abs(
                current.X -
                start.X);

        double height =
            Math.Abs(
                current.Y -
                start.Y);

        Canvas.SetLeft(
            DefectRectangle,
            left);

        Canvas.SetTop(
            DefectRectangle,
            top);

        DefectRectangle.Width =
            width;

        DefectRectangle.Height =
            height;

        DefectRectangle.Visibility =
            Visibility.Visible;
    }

    // ============================================================
    // FINISH DEFECT
    // ============================================================

    private void FinishDefectDrawing(
        MouseButtonEventArgs e)
    {
        if (!_isDrawingDefect ||
            e.ChangedButton !=
            MouseButton.Left)
        {
            return;
        }

        _defectCurrentPoint =
            GetMouseImagePoint(e);

        UpdateDefectRectangle();

        Point start =
            ClampPointToOverlay(
                _defectStartPoint);

        Point current =
            ClampPointToOverlay(
                _defectCurrentPoint);

        double left =
            Math.Min(
                start.X,
                current.X);

        double top =
            Math.Min(
                start.Y,
                current.Y);

        double displayWidth =
            Math.Abs(
                current.X -
                start.X);

        double displayHeight =
            Math.Abs(
                current.Y -
                start.Y);

        _isDrawingDefect = false;

        Mouse.Capture(null);

        ImageViewport.Cursor =
            Cursors.Arrow;

        ImagePanCanvas.Cursor =
            Cursors.Arrow;

        e.Handled = true;

        if (displayWidth < 5 ||
            displayHeight < 5)
        {
            HideTemporaryDefectRectangle();
            return;
        }

        if (DataContext is not ReviewViewModel viewModel ||
            viewModel.SelectedImage == null)
        {
            HideTemporaryDefectRectangle();
            return;
        }

        Point imageStart =
            OverlayPointToImage(
                new Point(
                    left,
                    top));

        double pixelWidth =
            DisplayWidthToImagePixels(
                displayWidth);

        double pixelHeight =
            DisplayHeightToImagePixels(
                displayHeight);

        try
        {
            DefectModel defect =
                DefectService.Instance.AddDefect(
                    viewModel.SelectedImage,
                    imageStart.X,
                    imageStart.Y,
                    pixelWidth,
                    pixelHeight);

            _selectedDefectId =
                defect.Id;

            HideTemporaryDefectRectangle();

            /*
             * Only the red rectangle is drawn.
             *
             * No dialog.
             * No detail card.
             * No text.
             */
            RefreshSavedDefects();
        }
        catch (Exception ex)
        {
            HideTemporaryDefectRectangle();

            MessageBox.Show(
                "Unable to create defect.\n\n" +
                ex.Message,
                "DEFECT",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void HideTemporaryDefectRectangle()
    {
        DefectRectangle.Visibility =
            Visibility.Collapsed;

        DefectRectangle.Width = 0;
        DefectRectangle.Height = 0;
    }

    // ============================================================
    // SAVED DEFECTS
    // ============================================================

    private void LoadSavedDefects()
    {
        ClearPersistedDefectRectangles();

        if (DataContext is not ReviewViewModel viewModel ||
            viewModel.SelectedImage == null)
        {
            return;
        }

        SyncDefectOverlay();

        var defects =
            DefectService.Instance.GetByImage(
                viewModel.SelectedImage.Id);

        foreach (DefectModel defect in defects)
        {
            AddPersistedDefectRectangle(
                defect);
        }
    }

    private void RefreshSavedDefects()
    {
        Dispatcher.BeginInvoke(
            new Action(LoadSavedDefects),
            DispatcherPriority.Render);
    }

    private void ClearPersistedDefectRectangles()
    {
        for (
            int i =
                DefectOverlayCanvas.Children.Count - 1;
            i >= 0;
            i--)
        {
            if (DefectOverlayCanvas.Children[i]
                is FrameworkElement element &&
                element.Tag is string tag &&
                tag.StartsWith(
                    PersistedDefectTagPrefix,
                    StringComparison.Ordinal))
            {
                DefectOverlayCanvas.Children.RemoveAt(i);
            }
        }
    }

    private void AddPersistedDefectRectangle(
        DefectModel defect)
    {
        bool selected =
            _selectedDefectId.HasValue &&
            _selectedDefectId.Value ==
            defect.Id;

        Point overlayPoint =
            ImagePointToOverlay(
                new Point(
                    defect.X,
                    defect.Y));

        double left =
            overlayPoint.X;

        double top =
            overlayPoint.Y;

        double width =
            Math.Max(
                1,
                ImagePixelWidthToDisplay(
                    defect.Width));

        double height =
            Math.Max(
                1,
                ImagePixelHeightToDisplay(
                    defect.Height));

        Rectangle rectangle =
            new Rectangle
            {
                Width = width,
                Height = height,

                Stroke =
                    new SolidColorBrush(
                        selected
                            ? Color.FromRgb(
                                255,
                                215,
                                0)
                            : Color.FromRgb(
                                255,
                                60,
                                60)),

                StrokeThickness =
                    selected ? 3 : 2,

                Fill =
                    new SolidColorBrush(
                        selected
                            ? Color.FromArgb(
                                35,
                                255,
                                215,
                                0)
                            : Color.FromArgb(
                                20,
                                255,
                                60,
                                60)),

                Cursor =
                    Cursors.Hand,

                IsHitTestVisible =
                    true,

                Tag =
                    PersistedDefectTagPrefix +
                    defect.Id
            };

        Canvas.SetLeft(
            rectangle,
            left);

        Canvas.SetTop(
            rectangle,
            top);

        Canvas.SetZIndex(
            rectangle,
            20);

        rectangle.MouseLeftButtonDown +=
            (_, args) =>
            {
                SelectDefect(
                    defect.Id);

                args.Handled = true;
            };

        DefectOverlayCanvas.Children.Add(
            rectangle);
    }

    // ============================================================
    // SELECT
    // ============================================================

    private void SelectDefect(
        Guid defectId)
    {
        _selectedDefectId =
            defectId;

        Focus();
        Keyboard.Focus(this);

        RefreshSavedDefects();
    }

    // ============================================================
    // DELETE
    // ============================================================

    private void DeleteSelectedDefect()
    {
        if (!_selectedDefectId.HasValue)
        {
            return;
        }

        Guid defectId =
            _selectedDefectId.Value;

        try
        {
            DefectService.Instance.RemoveDefect(
                defectId);

            _selectedDefectId = null;

            RefreshSavedDefects();

            Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    Focus();
                    Keyboard.Focus(this);
                }),
                DispatcherPriority.Input);
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Unable to delete the defect.\n\n" +
                ex.Message,
                "DELETE DEFECT",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    // ============================================================
    // COMBOBOX HANDLERS
    // ============================================================

    private void ComboBox_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
    }

    private void ComboBox_SelectionChanged_1(
        object sender,
        SelectionChangedEventArgs e)
    {
    }
}