using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class ReviewView : UserControl
{
    private bool _isPanning;
    private Point _lastMousePosition;

    private TranslateTransform? _panTransform;

    // DEFECT MARKING
    private bool _isDrawingDefect;
    private Point _defectStartPoint;
    private Point _defectCurrentPoint;

    public ReviewView()
    {
        InitializeComponent();

        DataContext = new ReviewViewModel();

        Loaded += ReviewView_Loaded;
        Unloaded += ReviewView_Unloaded;
    }

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

        ImageViewport.MouseWheel +=
            ImageViewport_MouseWheel;

        ImageViewport.SizeChanged +=
            ImageViewport_SizeChanged;

        if (DataContext is INotifyPropertyChanged notifyObject)
        {
            notifyObject.PropertyChanged +=
                ViewModel_PropertyChanged;
        }
    }

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

        ImageViewport.MouseWheel -=
            ImageViewport_MouseWheel;

        ImageViewport.SizeChanged -=
            ImageViewport_SizeChanged;

        if (_isPanning || _isDrawingDefect)
        {
            _isPanning = false;
            _isDrawingDefect = false;

            Mouse.Capture(null);
        }
    }

    private void ViewModel_PropertyChanged(
        object? sender,
        PropertyChangedEventArgs e)
    {
        if (!string.Equals(
                e.PropertyName,
                nameof(ReviewViewModel.ZoomLevel),
                StringComparison.Ordinal))
        {
            return;
        }

        Dispatcher.BeginInvoke(
            new Action(() =>
            {
                if (_panTransform == null)
                {
                    return;
                }

                double zoom =
                    GetZoom();

                if (zoom <= 1.001)
                {
                    ResetPan();
                }
                else
                {
                    ClampPan();
                }
            }),
            DispatcherPriority.Render);
    }

    // ============================================================
    // TRANSFORM
    // ============================================================

    private void SetupPanTransform()
    {
        if (ImagePanCanvas.RenderTransform
            is TransformGroup existingGroup)
        {
            foreach (Transform transform
                     in existingGroup.Children)
            {
                if (transform is TranslateTransform translate)
                {
                    _panTransform = translate;
                    return;
                }
            }

            _panTransform =
                new TranslateTransform();

            existingGroup.Children.Add(
                _panTransform);

            return;
        }

        if (ImagePanCanvas.RenderTransform
            is ScaleTransform existingScale)
        {
            TransformGroup scaleGroup =
                new TransformGroup();

            scaleGroup.Children.Add(
                existingScale);

            _panTransform =
                new TranslateTransform();

            scaleGroup.Children.Add(
                _panTransform);

            ImagePanCanvas.RenderTransform =
                scaleGroup;

            return;
        }

        TransformGroup newTransformGroup =
            new TransformGroup();

        newTransformGroup.Children.Add(
            new ScaleTransform(
                1.0,
                1.0));

        _panTransform =
            new TranslateTransform();

        newTransformGroup.Children.Add(
            _panTransform);

        ImagePanCanvas.RenderTransform =
            newTransformGroup;
    }

    // ============================================================
    // MOUSE DOWN
    // ============================================================

    private void ImagePanCanvas_MouseDown(
        object sender,
        MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            StartDefectDrawing(e);
            return;
        }

        StartPan(e);
    }

    private void ImageViewport_MouseDown(
        object sender,
        MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            StartDefectDrawing(e);
            return;
        }

        StartPan(e);
    }

    // ============================================================
    // PAN START
    // ============================================================

    private void StartPan(
        MouseButtonEventArgs e)
    {
        if (e.ChangedButton !=
            MouseButton.Middle)
        {
            return;
        }

        if (_panTransform == null)
        {
            return;
        }

        if (!IsZoomed())
        {
            return;
        }

        _isPanning = true;

        _lastMousePosition =
            e.GetPosition(ImageViewport);

        Mouse.Capture(
            ImageViewport,
            CaptureMode.SubTree);

        ImageViewport.Cursor =
            Cursors.Hand;

        ImagePanCanvas.Cursor =
            Cursors.Hand;

        e.Handled = true;
    }

    // ============================================================
    // DEFECT DRAWING
    // ============================================================

    private void StartDefectDrawing(
        MouseButtonEventArgs e)
    {
        if (e.ChangedButton !=
            MouseButton.Left)
        {
            return;
        }

        if (_isPanning)
        {
            return;
        }

        Point startPoint =
            e.GetPosition(ImagePanCanvas);

        if (startPoint.X < 0 ||
            startPoint.Y < 0 ||
            startPoint.X >
            ImagePanCanvas.ActualWidth ||
            startPoint.Y >
            ImagePanCanvas.ActualHeight)
        {
            return;
        }

        _isDrawingDefect = true;

        _defectStartPoint =
            ClampPointToCanvas(startPoint);

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

    private void UpdateDefectDrawing(
        MouseEventArgs e)
    {
        if (!_isDrawingDefect)
        {
            return;
        }

        Point currentPoint =
            e.GetPosition(ImagePanCanvas);

        _defectCurrentPoint =
            ClampPointToCanvas(currentPoint);

        UpdateDefectRectangle();

        e.Handled = true;
    }

    // ============================================================
    // DEFECT RECTANGLE
    // ============================================================

    private void PrepareDefectRectangle()
    {
        if (DefectRectangle == null)
        {
            return;
        }

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
        if (DefectRectangle == null)
        {
            return;
        }

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

    private Point ClampPointToCanvas(
        Point point)
    {
        double maxX =
            Math.Max(
                0,
                ImagePanCanvas.ActualWidth);

        double maxY =
            Math.Max(
                0,
                ImagePanCanvas.ActualHeight);

        return new Point(
            Math.Clamp(
                point.X,
                0,
                maxX),
            Math.Clamp(
                point.Y,
                0,
                maxY));
    }

    // ============================================================
    // MOUSE UP
    // ============================================================

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

    private void FinishDefectDrawing(
        MouseButtonEventArgs e)
    {
        if (!_isDrawingDefect)
        {
            return;
        }

        if (e.ChangedButton !=
            MouseButton.Left)
        {
            return;
        }

        Point finalPoint =
            e.GetPosition(ImagePanCanvas);

        _defectCurrentPoint =
            ClampPointToCanvas(finalPoint);

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

        // Ignore accidental tiny clicks.
        if (width < 5 ||
            height < 5)
        {
            if (DefectRectangle != null)
            {
                DefectRectangle.Visibility =
                    Visibility.Collapsed;
            }

            return;
        }

        // Keep final normalized rectangle.
        _defectStartPoint =
            new Point(
                left,
                top);

        _defectCurrentPoint =
            new Point(
                left + width,
                top + height);

        // ========================================================
        // CREATE DEFECT MODEL
        // ========================================================

        if (DataContext is not ReviewViewModel viewModel)
        {
            return;
        }

        ImageRecordModel? selectedImage =
            viewModel.SelectedImage;

        if (selectedImage == null)
        {
            return;
        }

        DefectService.Instance.AddDefect(
            selectedImage,
            left,
            top,
            width,
            height);
    }

    // ============================================================
    // PAN
    // ============================================================

    private void MovePan(
        MouseEventArgs e)
    {
        if (!_isPanning ||
            _panTransform == null)
        {
            return;
        }

        Point currentPosition =
            e.GetPosition(ImageViewport);

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

    // ============================================================
    // ZOOM
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

        Point mousePosition =
            e.GetPosition(ImageViewport);

        double relativeX =
            mousePosition.X -
            ImageViewport.ActualWidth / 2.0;

        double relativeY =
            mousePosition.Y -
            ImageViewport.ActualHeight / 2.0;

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
            new Action(ClampPan),
            DispatcherPriority.Render);

        e.Handled = true;
    }

    private static void SetZoomFromView(
        ReviewViewModel viewModel,
        double zoom)
    {
        if (Math.Abs(
                viewModel.ZoomLevel - zoom) <
            0.001)
        {
            return;
        }

        if (zoom >
            viewModel.ZoomLevel)
        {
            while (viewModel.ZoomLevel <
                   zoom - 0.001)
            {
                viewModel.ZoomInCommand.Execute(
                    null);
            }
        }
        else
        {
            while (viewModel.ZoomLevel >
                   zoom + 0.001)
            {
                viewModel.ZoomOutCommand.Execute(
                    null);
            }
        }
    }

    // ============================================================
    // PAN END
    // ============================================================

    private void EndPan(
        MouseButtonEventArgs e)
    {
        if (e.ChangedButton !=
            MouseButton.Middle)
        {
            return;
        }

        if (!_isPanning)
        {
            return;
        }

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
    // VIEWPORT
    // ============================================================

    private void ImageViewport_SizeChanged(
        object sender,
        SizeChangedEventArgs e)
    {
        ClampPan();
    }

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

    private void ResetPan()
    {
        if (_panTransform == null)
        {
            return;
        }

        _panTransform.X = 0;
        _panTransform.Y = 0;

        _isPanning = false;

        if (Mouse.Captured ==
            ImageViewport)
        {
            Mouse.Capture(null);
        }

        ImageViewport.Cursor =
            Cursors.Arrow;

        ImagePanCanvas.Cursor =
            Cursors.Arrow;
    }

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
            ResetPan();
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
    // COMBOBOX
    // ============================================================

    private void ComboBox_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
    }
}