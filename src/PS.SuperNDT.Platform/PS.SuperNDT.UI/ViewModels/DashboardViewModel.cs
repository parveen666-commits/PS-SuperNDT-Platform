using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class DashboardViewModel : INotifyPropertyChanged
{
    private readonly DispatcherTimer _timer;

    private readonly WorkOrderService _workOrderService;
    private readonly ImageService _imageService;

    private string _currentJob = "No Active Work Order";
    private string _customer = "-";
    private int _totalImages;
    private int _totalJobs;
    private int _openJobs;
    private int _closedJobs;

    public string CurrentJob
    {
        get => _currentJob;
        set
        {
            if (_currentJob == value)
                return;

            _currentJob = value;
            OnPropertyChanged();
        }
    }

    public string Customer
    {
        get => _customer;
        set
        {
            if (_customer == value)
                return;

            _customer = value;
            OnPropertyChanged();
        }
    }

    public int TotalImages
    {
        get => _totalImages;
        set
        {
            if (_totalImages == value)
                return;

            _totalImages = value;
            OnPropertyChanged();
        }
    }

    public int TotalJobs
    {
        get => _totalJobs;
        set
        {
            if (_totalJobs == value)
                return;

            _totalJobs = value;
            OnPropertyChanged();
        }
    }

    public int OpenJobs
    {
        get => _openJobs;
        set
        {
            if (_openJobs == value)
                return;

            _openJobs = value;
            OnPropertyChanged();
        }
    }

    public int ClosedJobs
    {
        get => _closedJobs;
        set
        {
            if (_closedJobs == value)
                return;

            _closedJobs = value;
            OnPropertyChanged();
        }
    }

    public DashboardViewModel()
    {
        _workOrderService = new WorkOrderService();
        _imageService = new ImageService();

        Refresh();

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(2)
        };

        _timer.Tick += (_, _) => Refresh();
        _timer.Start();
    }

    private void Refresh()
    {
        try
        {
            RefreshCurrentWorkOrder();
            RefreshImageCount();
            RefreshWorkOrderCounts();
        }
        catch
        {
            // Dashboard refresh must never stop the UI timer.
        }
    }

    private void RefreshCurrentWorkOrder()
    {
        var currentJobService = CurrentJobService.Instance;

        var currentWorkOrder = currentJobService.CurrentWorkOrder;
        var currentJob = currentJobService.CurrentJob;

        CurrentJob =
            currentWorkOrder?.WorkOrderNumber ??
            currentJob?.JobNumber ??
            "No Active Work Order";

        Customer =
            currentWorkOrder?.Customer ??
            currentJob?.Customer ??
            "-";
    }

    private void RefreshImageCount()
    {
        TotalImages = _imageService.GetTotalImageCount();
    }

    private void RefreshWorkOrderCounts()
    {
        var workOrders = _workOrderService
            .GetAll()
            .ToList();

        TotalJobs = workOrders.Count;

        ClosedJobs = workOrders.Count(IsClosedWorkOrder);

        OpenJobs = workOrders.Count(x => !IsClosedWorkOrder(x));
    }

    private static bool IsClosedWorkOrder(Models.WorkOrderModel workOrder)
    {
        return workOrder.IsClosed ||
               string.Equals(
                   workOrder.Status,
                   "CLOSED",
                   StringComparison.OrdinalIgnoreCase);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(
        [CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}