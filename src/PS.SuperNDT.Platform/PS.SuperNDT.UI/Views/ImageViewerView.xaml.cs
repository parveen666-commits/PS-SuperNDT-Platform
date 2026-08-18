using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class ImageViewerView : UserControl
{
    private Point _panStartPoint;
    private double _panStartHorizontalOffset;
    private double _panStartVerticalOffset;
    private bool _isPanning;

    public ImageViewerView()
    {
        InitializeComponent();

        DataContext =
            new ImageViewerViewModel();

        PreviewMouseWheel +=
            ImageViewerView_PreviewMouseWheel;

        PreviewMouseDown +=
            ImageViewerView_PreviewMouseDown;

        PreviewMouseMove +=
            ImageViewerView_PreviewMouseMove;

        PreviewMouseUp +=
            ImageViewerView_PreviewMouseUp;
    }

    private void ImageViewerView_PreviewMouseWheel(
        object sender,
        MouseWheelEventArgs e)
    {
        if (DataContext is not ImageViewerViewModel viewModel)
            return;

        if (e.Delta > 0)
        {
            viewModel.ZoomInCommand.Execute(null);
        }
        else if (e.Delta < 0)
        {
            viewModel.ZoomOutCommand.Execute(null);
        }

        e.Handled = true;
    }

    private void ImageViewerView_PreviewMouseDown(
        object sender,
        MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Middle)
        {
            StartPanning(e);
            return;
        }

        if (e.ChangedButton == MouseButton.Left)
        {
            HandleImageClick(e);
        }
    }

    private void StartPanning(
        MouseButtonEventArgs e)
    {
        ScrollViewer? scrollViewer =
            FindVisualChild<ScrollViewer>(this);

        if (scrollViewer == null)
            return;

        _isPanning = true;

        _panStartPoint =
            e.GetPosition(this);

        _panStartHorizontalOffset =
            scrollViewer.HorizontalOffset;

        _panStartVerticalOffset =
            scrollViewer.VerticalOffset;

        Cursor =
            Cursors.Hand;

        CaptureMouse();

        e.Handled = true;
    }

    private void HandleImageClick(
        MouseButtonEventArgs e)
    {
        if (DataContext is not ImageViewerViewModel viewModel)
            return;

        Image? image =
            FindVisualChild<Image>(this);

        if (image == null ||
            image.Source == null)
        {
            return;
        }

        Point displayedPoint =
            e.GetPosition(image);

        double displayedWidth =
            image.ActualWidth;

        double displayedHeight =
            image.ActualHeight;

        if (displayedWidth <= 0 ||
            displayedHeight <= 0)
        {
            return;
        }

        if (displayedPoint.X < 0 ||
            displayedPoint.Y < 0 ||
            displayedPoint.X > displayedWidth ||
            displayedPoint.Y > displayedHeight)
        {
            return;
        }

        /*
         * Convert displayed coordinates back to
         * image coordinates so zoom does not change
         * the measured pixel distance.
         */

        double zoom =
            viewModel.ZoomScale;

        if (zoom <= 0)
            zoom = 1.0;

        Point imagePoint =
            new Point(
                displayedPoint.X / zoom,
                displayedPoint.Y / zoom);

        /*
         * Calibration has priority over normal
         * measurement.
         */

        if (viewModel.IsCalibrationMode)
        {
            HandleCalibrationClick(
                viewModel,
                imagePoint);

            e.Handled = true;
            return;
        }

        if (viewModel.IsMeasurementMode)
        {
            HandleMeasurementClick(
                viewModel,
                imagePoint);

            e.Handled = true;
        }
    }

    private static void HandleCalibrationClick(
        ImageViewerViewModel viewModel,
        Point imagePoint)
    {
        if (!viewModel.CalibrationStartPoint.HasValue)
        {
            viewModel.SetCalibrationStartPoint(
                imagePoint);

            return;
        }

        if (!viewModel.CalibrationEndPoint.HasValue)
        {
            viewModel.SetCalibrationEndPoint(
                imagePoint);

            return;
        }

        /*
         * Third click starts a new calibration line.
         */

        viewModel.SetCalibrationStartPoint(
            imagePoint);
    }

    private static void HandleMeasurementClick(
        ImageViewerViewModel viewModel,
        Point imagePoint)
    {
        if (!viewModel.MeasurementStartPoint.HasValue)
        {
            viewModel.SetMeasurementStartPoint(
                imagePoint);

            return;
        }

        if (!viewModel.MeasurementEndPoint.HasValue)
        {
            viewModel.SetMeasurementEndPoint(
                imagePoint);

            return;
        }

        /*
         * Third click starts a new measurement.
         */

        viewModel.SetMeasurementStartPoint(
            imagePoint);
    }

    private void ImageViewerView_PreviewMouseMove(
        object sender,
        MouseEventArgs e)
    {
        if (!_isPanning)
            return;

        if (e.MiddleButton !=
            MouseButtonState.Pressed)
        {
            StopPanning();
            return;
        }

        ScrollViewer? scrollViewer =
            FindVisualChild<ScrollViewer>(this);

        if (scrollViewer == null)
            return;

        Point currentPoint =
            e.GetPosition(this);

        double horizontalDelta =
            currentPoint.X -
            _panStartPoint.X;

        double verticalDelta =
            currentPoint.Y -
            _panStartPoint.Y;

        scrollViewer.ScrollToHorizontalOffset(
            _panStartHorizontalOffset -
            horizontalDelta);

        scrollViewer.ScrollToVerticalOffset(
            _panStartVerticalOffset -
            verticalDelta);

        e.Handled = true;
    }

    private void ImageViewerView_PreviewMouseUp(
        object sender,
        MouseButtonEventArgs e)
    {
        if (e.ChangedButton !=
            MouseButton.Middle)
        {
            return;
        }

        StopPanning();

        e.Handled = true;
    }

    private void StopPanning()
    {
        if (!_isPanning)
            return;

        _isPanning = false;

        if (IsMouseCaptured)
            ReleaseMouseCapture();

        Cursor =
            Cursors.Arrow;
    }

    private static T? FindVisualChild<T>(
        DependencyObject parent)
        where T : DependencyObject
    {
        if (parent == null)
            return null;

        int childCount =
            VisualTreeHelper.GetChildrenCount(
                parent);

        for (int i = 0;
             i < childCount;
             i++)
        {
            DependencyObject child =
                VisualTreeHelper.GetChild(
                    parent,
                    i);

            if (child is T result)
                return result;

            T? descendant =
                FindVisualChild<T>(child);

            if (descendant != null)
                return descendant;
        }

        return null;
    }
}