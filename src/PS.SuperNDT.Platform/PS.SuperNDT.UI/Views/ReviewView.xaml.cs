using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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

    private const string PersistedDefectDetailTagPrefix =
        "PERSISTED_DEFECT_DETAIL:";

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

        Focusable = true;

        SetupPanTransform();

        ImagePanCanvas.MouseDown +=
            ImagePanCanvas_MouseDown;

        ImagePanCanvas.MouseMove +=
            ImagePanCanvas_MouseMove;

        ImagePanCanvas.MouseUp +=
            ImagePanCanvas_MouseUp;

        ImagePanCanvas.MouseLeave +=
            ImagePanCanvas_MouseLeave;

        ImageViewport.MouseDown +=
            ImageViewport_MouseDown;

        ImageViewport.MouseMove +=
            ImageViewport_MouseMove;

        ImageViewport.MouseUp +=
            ImageViewport_MouseUp;

        ImageViewport.MouseLeave +=
            ImageViewport_MouseLeave;

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
                LoadSavedDefects();
                UpdateRulers();
            }),
            DispatcherPriority.Render);
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

        ImagePanCanvas.MouseDown -=
            ImagePanCanvas_MouseDown;

        ImagePanCanvas.MouseMove -=
            ImagePanCanvas_MouseMove;

        ImagePanCanvas.MouseUp -=
            ImagePanCanvas_MouseUp;

        ImagePanCanvas.MouseLeave -=
            ImagePanCanvas_MouseLeave;

        ImageViewport.MouseDown -=
            ImageViewport_MouseDown;

        ImageViewport.MouseMove -=
            ImageViewport_MouseMove;

        ImageViewport.MouseUp -=
            ImageViewport_MouseUp;

        ImageViewport.MouseLeave -=
            ImageViewport_MouseLeave;

        ImageViewport.PreviewMouseWheel -=
            ImageViewport_MouseWheel;

        ImageViewport.SizeChanged -=
            ImageViewport_SizeChanged;

        ClearPersistedDefectRectangles();
        ClearDynamicRulers();

        _selectedDefectId = null;

        _isPanning = false;
        _isDrawingDefect = false;

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

                    double zoom =
                        GetZoom();

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

                    UpdateRulers();
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

                    double zoom =
                        GetZoom();

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

                    RefreshSavedDefects();
                    UpdateRulers();
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

            ImageViewport.Cursor =
                Cursors.Arrow;

            ImagePanCanvas.Cursor =
                Cursors.Arrow;

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
                _scaleTransform.ScaleX =
                    FitZoom;

                _scaleTransform.ScaleY =
                    FitZoom;
            }
        }
        finally
        {
            _isFittingFrame = false;
        }

        UpdateRulers();
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

        _scaleTransform.ScaleX =
            zoom;

        _scaleTransform.ScaleY =
            zoom;

        UpdateRulers();
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
    // TRANSFORMS
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
                    _scaleTransform =
                        scale;
                }

                if (transform is TranslateTransform translate)
                {
                    _panTransform =
                        translate;
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
            double zoom =
                GetZoom();

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

                ImageViewport.Cursor =
                    Cursors.Arrow;

                ImagePanCanvas.Cursor =
                    Cursors.Arrow;
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

        UpdateRulers();
    }

    // ============================================================
    // RULER
    // ============================================================

    private void UpdateRulers()
    {
        if (!IsLoaded ||
            TopRulerCanvas == null ||
            RulerCanvas == null)
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
            RulerCanvas.ActualWidth;

        if (rulerWidth <= 1)
        {
            rulerWidth =
                ShotFrame.ActualWidth;
        }

        if (rulerWidth <= 1)
        {
            return;
        }

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
                        current % 50.0) <
                    0.001;

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

                Tag =
                    RulerTickTag
            };

        RulerCanvas.Children.Add(
            tick);

        if (!major)
        {
            return;
        }

        TextBlock label =
            new TextBlock
            {
                Text =
                    $"{value:0}",

                Foreground =
                    new SolidColorBrush(
                        Color.FromRgb(
                            227,
                            232,
                            238)),

                FontSize = 9,

                IsHitTestVisible = false,

                Tag =
                    RulerTickTag
            };

        Canvas.SetLeft(
            label,
            x + 2);

        Canvas.SetTop(
            label,
            18);

        RulerCanvas.Children.Add(
            label);
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

                Tag =
                    RulerTickTag
            };

        TopRulerCanvas.Children.Add(
            tick);

        if (!major)
        {
            return;
        }

        TextBlock label =
            new TextBlock
            {
                Text =
                    $"{value:0}",

                Foreground =
                    new SolidColorBrush(
                        Color.FromRgb(
                            227,
                            232,
                            238)),

                FontSize = 9,

                IsHitTestVisible = false,

                Tag =
                    RulerTickTag
            };

        Canvas.SetLeft(
            label,
            x + 2);

        Canvas.SetTop(
            label,
            1);

        TopRulerCanvas.Children.Add(
            label);
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
                    UpdateRulers();
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

                UpdateRulers();
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
                    UpdateRulers();
                }),
                DispatcherPriority.Render);
        }
        else
        {
            ApplyZoomVisual();
            UpdateScrollMode();
            ClampPan();
            UpdateRulers();
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

        Point point =
            e.GetPosition(
                DefectOverlayCanvas);

        _isDrawingDefect = true;

        _defectStartPoint =
            ClampPointToOverlay(point);

        _defectCurrentPoint =
            _defectStartPoint;

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

        Point point =
            e.GetPosition(
                DefectOverlayCanvas);

        _defectCurrentPoint =
            ClampPointToOverlay(point);

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
        double left =
            Math.Min(
                _defectStartPoint.X,
                _defectCurrentPoint.X);

        double top =
            Math.Min(
                _defectStartPoint.Y,
                _defectCurrentPoint.Y);

        double width =
            Math.Abs(
                _defectCurrentPoint.X -
                _defectStartPoint.X);

        double height =
            Math.Abs(
                _defectCurrentPoint.Y -
                _defectStartPoint.Y);

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

        Point point =
            e.GetPosition(
                DefectOverlayCanvas);

        _defectCurrentPoint =
            ClampPointToOverlay(point);

        UpdateDefectRectangle();

        double left =
            Math.Min(
                _defectStartPoint.X,
                _defectCurrentPoint.X);

        double top =
            Math.Min(
                _defectStartPoint.Y,
                _defectCurrentPoint.Y);

        double width =
            Math.Abs(
                _defectCurrentPoint.X -
                _defectStartPoint.X);

        double height =
            Math.Abs(
                _defectCurrentPoint.Y -
                _defectStartPoint.Y);

        _isDrawingDefect = false;

        Mouse.Capture(null);

        ImageViewport.Cursor =
            Cursors.Arrow;

        ImagePanCanvas.Cursor =
            Cursors.Arrow;

        e.Handled = true;

        if (width < 5 ||
            height < 5)
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

        DefectModel defect;

        try
        {
            defect =
                DefectService.Instance.AddDefect(
                    viewModel.SelectedImage,
                    left,
                    top,
                    width,
                    height);
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

            return;
        }

        _selectedDefectId =
            defect.Id;

        HideTemporaryDefectRectangle();

        OpenDefectDetailsDialog(
            defect);
    }

    // ============================================================
    // DEFECT DETAILS DIALOG
    // ============================================================

    private void OpenDefectDetailsDialog(
        DefectModel defect)
    {
        DefectDialog dialog =
            new DefectDialog(
                defect.PipePosition,
                defect.LengthMm > 0
                    ? defect.LengthMm
                    : 1,
                defect.WidthMm > 0
                    ? defect.WidthMm
                    : 1);

        Window? owner =
            Window.GetWindow(this);

        if (owner != null)
        {
            dialog.Owner =
                owner;
        }

        bool? result =
            dialog.ShowDialog();

        if (result != true)
        {
            try
            {
                DefectService.Instance.RemoveDefect(
                    defect.Id);
            }
            catch
            {
            }

            _selectedDefectId = null;

            RefreshSavedDefects();

            return;
        }

        try
        {
            defect.DefectType =
                string.IsNullOrWhiteSpace(
                    dialog.DefectType)
                    ? "UNCLASSIFIED"
                    : dialog.DefectType.Trim();

            defect.Severity =
                string.IsNullOrWhiteSpace(
                    dialog.Severity)
                    ? "UNCLASSIFIED"
                    : dialog.Severity.Trim();

            defect.PipePosition =
                Math.Max(
                    0,
                    dialog.Position);

            defect.LengthMm =
                Math.Max(
                    0,
                    dialog.Length);

            defect.WidthMm =
                Math.Max(
                    0,
                    dialog.DefectWidth);

            defect.Description =
                dialog.Remarks;

            DefectService.Instance.UpdateDefect(
                defect);

            _selectedDefectId =
                defect.Id;

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
                "Unable to save defect details.\n\n" +
                ex.Message,
                "DEFECT DETAILS",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            RefreshSavedDefects();
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
                (
                    tag.StartsWith(
                        PersistedDefectTagPrefix,
                        StringComparison.Ordinal) ||
                    tag.StartsWith(
                        PersistedDefectDetailTagPrefix,
                        StringComparison.Ordinal)
                ))
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

        Rectangle rectangle =
            new Rectangle
            {
                Width =
                    Math.Max(
                        1,
                        defect.Width),

                Height =
                    Math.Max(
                        1,
                        defect.Height),

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
                                45,
                                255,
                                215,
                                0)
                            : Color.FromArgb(
                                35,
                                255,
                                60,
                                60)),

                Cursor =
                    Cursors.Hand,

                IsHitTestVisible = true,

                Tag =
                    PersistedDefectTagPrefix +
                    defect.Id
            };

        Canvas.SetLeft(
            rectangle,
            defect.X);

        Canvas.SetTop(
            rectangle,
            defect.Y);

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

        Border detailCard =
            CreateDefectDetailCard(
                defect);

        Canvas.SetLeft(
            detailCard,
            GetDefectDetailLeft(
                defect));

        Canvas.SetTop(
            detailCard,
            GetDefectDetailTop(
                defect));

        Canvas.SetZIndex(
            detailCard,
            30);

        DefectOverlayCanvas.Children.Add(
            detailCard);
    }

    // ============================================================
    // DEFECT DETAIL CARD
    // ============================================================

    private Border CreateDefectDetailCard(
        DefectModel defect)
    {
        StackPanel panel =
            new StackPanel();

        string type =
            string.IsNullOrWhiteSpace(
                defect.DefectType)
                ? "UNCLASSIFIED"
                : defect.DefectType;

        string severity =
            string.IsNullOrWhiteSpace(
                defect.Severity)
                ? "UNCLASSIFIED"
                : defect.Severity;

        panel.Children.Add(
            CreateInfo(
                $"DEFECT  •  {type}",
                true));

        panel.Children.Add(
            CreateInfo(
                $"POS       {defect.PipePosition:0.0} mm"));

        panel.Children.Add(
            CreateInfo(
                $"LENGTH    {defect.LengthMm:0.0} mm"));

        panel.Children.Add(
            CreateInfo(
                $"WIDTH     {defect.WidthMm:0.0} mm"));

        panel.Children.Add(
            CreateInfo(
                $"SEVERITY  {severity}"));

        if (!string.IsNullOrWhiteSpace(
                defect.Description))
        {
            TextBlock remark =
                CreateInfo(
                    $"REMARK    {defect.Description}");

            remark.TextWrapping =
                TextWrapping.Wrap;

            panel.Children.Add(
                remark);
        }

        bool selected =
            _selectedDefectId.HasValue &&
            _selectedDefectId.Value ==
            defect.Id;

        Border card =
            new Border
            {
                Width = 255,

                Background =
                    new SolidColorBrush(
                        Color.FromArgb(
                            245,
                            20,
                            24,
                            31)),

                BorderBrush =
                    new SolidColorBrush(
                        selected
                            ? Color.FromRgb(
                                255,
                                215,
                                0)
                            : Color.FromRgb(
                                255,
                                75,
                                75)),

                BorderThickness =
                    new Thickness(
                        selected ? 2 : 1),

                CornerRadius =
                    new CornerRadius(4),

                Padding =
                    new Thickness(
                        8,
                        6,
                        8,
                        6),

                Child =
                    panel,

                Cursor =
                    Cursors.Hand,

                IsHitTestVisible =
                    true,

                Tag =
                    PersistedDefectDetailTagPrefix +
                    defect.Id
            };

        card.MouseLeftButtonDown +=
            (_, args) =>
            {
                SelectDefect(
                    defect.Id);

                args.Handled = true;
            };

        return card;
    }

    private static TextBlock CreateInfo(
        string text,
        bool bold = false)
    {
        return new TextBlock
        {
            Text = text,

            Foreground =
                new SolidColorBrush(
                    Color.FromRgb(
                        225,
                        230,
                        236)),

            FontSize = 10,

            FontWeight =
                bold
                    ? FontWeights.Bold
                    : FontWeights.Normal,

            Margin =
                new Thickness(
                    0,
                    1,
                    0,
                    1)
        };
    }

    private static double GetDefectDetailLeft(
        DefectModel defect)
    {
        return Math.Max(
            5,
            defect.X);
    }

    private static double GetDefectDetailTop(
        DefectModel defect)
    {
        double top =
            defect.Y - 110;

        if (top < 5)
        {
            top =
                defect.Y +
                Math.Max(
                    1,
                    defect.Height) +
                6;
        }

        return top;
    }

    // ============================================================
    // SELECT DEFECT
    // ============================================================

    private void SelectDefect(
        Guid defectId)
    {
        _selectedDefectId =
            defectId;

        // Keep keyboard focus on ReviewView so that
        // Delete is received reliably after clicking
        // a defect rectangle or detail card.
        Focus();
        Keyboard.Focus(this);

        RefreshSavedDefects();
    }

    // ============================================================
    // DELETE SELECTED DEFECT
    // ============================================================

    private void DeleteSelectedDefect()
    {
        if (!_selectedDefectId.HasValue)
        {
            return;
        }

        Guid defectId =
            _selectedDefectId.Value;

        DefectModel? defect =
            DefectService.Instance.GetById(
                defectId);

        if (defect == null)
        {
            _selectedDefectId = null;

            RefreshSavedDefects();

            return;
        }

        MessageBoxResult result =
            MessageBox.Show(
                "Are you sure you want to delete this defect?\n\n" +
                $"Type: {defect.DefectType}\n" +
                $"Position: {defect.PipePosition:0.0} mm",
                "DELETE DEFECT",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes)
        {
            return;
        }

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
    // EMPTY HANDLERS
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