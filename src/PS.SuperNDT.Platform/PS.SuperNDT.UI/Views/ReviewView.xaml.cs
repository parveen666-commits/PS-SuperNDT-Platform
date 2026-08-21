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

    private bool _isPanning;
    private Point _lastMousePosition;

    private TranslateTransform? _panTransform;
    private ScaleTransform? _scaleTransform;

    private bool _isDrawingDefect;
    private Point _defectStartPoint;
    private Point _defectCurrentPoint;

    private bool _updatingScrollMode;
    private bool _isFittingFrame;

    public ReviewView()
    {
        InitializeComponent();

        DataContext = new ReviewViewModel();

        Loaded += ReviewView_Loaded;
        Unloaded += ReviewView_Unloaded;

        PreviewKeyDown += ReviewView_PreviewKeyDown;
        PreviewMouseWheel += ReviewView_PreviewMouseWheel;
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
                UpdateScrollMode();
                FitImageToFrame();
                LoadSavedDefects();
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

        PreviewKeyDown -=
            ReviewView_PreviewKeyDown;

        PreviewMouseWheel -=
            ReviewView_PreviewMouseWheel;

        ClearPersistedDefectRectangles();

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
                    double zoom =
                        GetZoom();

                    if (zoom <= 1.001)
                    {
                        FitImageToFrame();
                    }
                    else
                    {
                        UpdateScrollMode();
                        ClampPan();
                    }
                }),
                DispatcherPriority.Render);

            return;
        }

        if (string.Equals(
                e.PropertyName,
                nameof(ReviewViewModel.SelectedImage),
                StringComparison.Ordinal))
        {
            Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    if (GetZoom() <= 1.001)
                    {
                        FitImageToFrame();
                    }

                    RefreshSavedDefects();
                }),
                DispatcherPriority.Render);
        }

        if (string.Equals(
                e.PropertyName,
                nameof(ReviewViewModel.DisplayImage),
                StringComparison.Ordinal))
        {
            Dispatcher.BeginInvoke(
                new Action(() =>
                {
                    if (GetZoom() <= 1.001)
                    {
                        FitImageToFrame();
                    }

                    RefreshSavedDefects();
                }),
                DispatcherPriority.Render);
        }
    }

    // ============================================================
    // FIT TO FRAME
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

            /*
             * At 1.00x the canvas itself becomes exactly the
             * available viewport. The Image uses Uniform so the
             * complete image/ruler area remains inside one frame.
             */
            double viewportWidth =
                Math.Max(
                    1,
                    ImageViewport.ActualWidth - 4);

            double viewportHeight =
                Math.Max(
                    1,
                    ImageViewport.ActualHeight - 4);

            ImagePanCanvas.Width =
                viewportWidth;

            ImagePanCanvas.Height =
                viewportHeight;

            if (_scaleTransform != null)
            {
                _scaleTransform.ScaleX = 1.0;
                _scaleTransform.ScaleY = 1.0;
            }

            UpdateScrollMode();

            ImageViewport.ScrollToHorizontalOffset(0);
            ImageViewport.ScrollToVerticalOffset(0);
        }
        finally
        {
            _isFittingFrame = false;
        }
    }

    // ============================================================
    // SCROLL MODE
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

            if (zoom <= 1.001)
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
                        1.0,
                        1.0);

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
                1.0,
                1.0);

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
    // KEYBOARD
    // ============================================================

    private void ReviewView_PreviewKeyDown(
        object sender,
        KeyEventArgs e)
    {
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
                new Action(FitImageToFrame),
                DispatcherPriority.Render);

            e.Handled = true;
        }
    }

    // ============================================================
    // MOUSE WHEEL ZOOM
    // ============================================================

    private void ReviewView_PreviewMouseWheel(
        object sender,
        MouseWheelEventArgs e)
    {
        HandleZoomWheel(e);
    }

    private void ImageViewport_MouseWheel(
        object sender,
        MouseWheelEventArgs e)
    {
        HandleZoomWheel(e);
    }

    private void HandleZoomWheel(
        MouseWheelEventArgs e)
    {
        if (DataContext is not ReviewViewModel viewModel)
        {
            return;
        }

        double oldZoom =
            viewModel.ZoomLevel;

        double newZoom;

        if (e.Delta > 0)
        {
            newZoom =
                Math.Min(
                    5.0,
                    oldZoom + 0.25);
        }
        else
        {
            newZoom =
                Math.Max(
                    0.25,
                    oldZoom - 0.25);
        }

        if (Math.Abs(
                oldZoom - newZoom) < 0.001)
        {
            e.Handled = true;

            return;
        }

        if (newZoom <= 1.001)
        {
            viewModel.ResetZoomCommand.Execute(null);

            Dispatcher.BeginInvoke(
                new Action(FitImageToFrame),
                DispatcherPriority.Render);

            e.Handled = true;

            return;
        }

        if (_panTransform == null ||
            _scaleTransform == null)
        {
            SetupPanTransform();
        }

        Point mousePosition =
            e.GetPosition(
                ImageViewport);

        double centerX =
            ImageViewport.ActualWidth / 2.0;

        double centerY =
            ImageViewport.ActualHeight / 2.0;

        double relativeX =
            mousePosition.X - centerX;

        double relativeY =
            mousePosition.Y - centerY;

        double oldPanX =
            _panTransform?.X ?? 0;

        double oldPanY =
            _panTransform?.Y ?? 0;

        double zoomRatio =
            newZoom / oldZoom;

        if (_panTransform != null)
        {
            _panTransform.X =
                relativeX -
                (relativeX - oldPanX) *
                zoomRatio;

            _panTransform.Y =
                relativeY -
                (relativeY - oldPanY) *
                zoomRatio;
        }

        SetZoomFromView(
            viewModel,
            newZoom);

        Dispatcher.BeginInvoke(
            new Action(() =>
            {
                UpdateScrollMode();
                ClampPan();
            }),
            DispatcherPriority.Render);

        e.Handled = true;
    }

    private static void SetZoomFromView(
        ReviewViewModel viewModel,
        double zoom)
    {
        if (Math.Abs(
                viewModel.ZoomLevel - zoom) < 0.001)
        {
            return;
        }

        if (zoom > viewModel.ZoomLevel)
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
    // PAN
    // ============================================================

    private bool IsZoomed()
    {
        return GetZoom() > 1.001;
    }

    private double GetZoom()
    {
        if (DataContext is ReviewViewModel viewModel)
        {
            return viewModel.ZoomLevel;
        }

        return 1.0;
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

    private void ImagePanCanvas_MouseMove(
        object sender,
        MouseEventArgs e)
    {
        if (_isDrawingDefect)
        {
            UpdateDefectDrawing(e);

            return;
        }

        if (!_isPanning)
        {
            return;
        }

        MovePan(e);
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

        if (!_isPanning)
        {
            return;
        }

        MovePan(e);
    }

    private void MovePan(
        MouseEventArgs e)
    {
        if (!_isPanning ||
            _panTransform == null ||
            !IsZoomed())
        {
            return;
        }

        Point currentPosition =
            e.GetPosition(
                ImageViewport);

        double deltaX =
            currentPosition.X -
            _lastMousePosition.X;

        double deltaY =
            currentPosition.Y -
            _lastMousePosition.Y;

        _lastMousePosition =
            currentPosition;

        _panTransform.X +=
            deltaX;

        _panTransform.Y +=
            deltaY;

        ClampPan();

        e.Handled = true;
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

    private void ImagePanCanvas_MouseLeave(
        object sender,
        MouseEventArgs e)
    {
        if (_isPanning)
        {
            ImageViewport.Cursor =
                Cursors.Hand;
        }
        else if (_isDrawingDefect)
        {
            ImageViewport.Cursor =
                Cursors.Cross;
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
        else if (_isDrawingDefect)
        {
            ImageViewport.Cursor =
                Cursors.Cross;
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

        if (zoom <= 1.001)
        {
            _panTransform.X = 0;
            _panTransform.Y = 0;

            ImageViewport.ScrollToHorizontalOffset(0);
            ImageViewport.ScrollToVerticalOffset(0);

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

        if (canvasWidth <= 0 ||
            canvasHeight <= 0 ||
            viewportWidth <= 0 ||
            viewportHeight <= 0)
        {
            return;
        }

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
        if (GetZoom() <= 1.001)
        {
            Dispatcher.BeginInvoke(
                new Action(FitImageToFrame),
                DispatcherPriority.Render);
        }
        else
        {
            UpdateScrollMode();
            ClampPan();
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

        point =
            ClampPointToOverlay(point);

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

        Point point =
            e.GetPosition(
                DefectOverlayCanvas);

        _defectCurrentPoint =
            ClampPointToOverlay(point);

        UpdateDefectRectangle();

        e.Handled = true;
    }

    private void PrepareDefectRectangle()
    {
        DefectRectangle.Visibility =
            Visibility.Visible;

        DefectRectangle.Width = 0;
        DefectRectangle.Height = 0;

        Canvas.SetLeft(
            DefectRectangle,
            _defectStartPoint.X);

        Canvas.SetTop(
            DefectRectangle,
            _defectStartPoint.Y);
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

        DefectService.Instance.AddDefect(
            viewModel.SelectedImage,
            left,
            top,
            width,
            height);

        HideTemporaryDefectRectangle();

        RefreshSavedDefects();
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
            AddPersistedDefectRectangle(defect);
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
                        Color.FromRgb(
                            255,
                            60,
                            60)),

                StrokeThickness = 2,

                Fill =
                    new SolidColorBrush(
                        Color.FromArgb(
                            35,
                            255,
                            60,
                            60)),

                IsHitTestVisible = false,

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

        DefectOverlayCanvas.Children.Add(
            rectangle);
    }

    // ============================================================
    // EMPTY HANDLER
    // ============================================================

    private void ComboBox_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
    }
}