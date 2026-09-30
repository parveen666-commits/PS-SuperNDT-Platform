
﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using PS.SuperNDT.UI.Commands;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;
using PS.SuperNDT.UI.Views;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class JobHistoryViewModel : INotifyPropertyChanged
{
    private readonly WorkOrderService _workOrderService;
    private readonly JobService _jobService;
    private readonly ImageService _imageService;

    private string _searchText = string.Empty;
    private string _selectedStatus = "ALL";
    private string _selectedOperator = "ALL";
    private string _selectedCustomer = "ALL";

    private JobHistoryRowModel? _selectedJob;

    public ObservableCollection<JobHistoryRowModel> Jobs { get; } = new();

    public ObservableCollection<JobHistoryRowModel> FilteredJobs { get; } = new();

    public ObservableCollection<string> StatusItems { get; } =
        new()
        {
            "ALL",
            "OPEN",
            "IN PROGRESS",
            "RT",
            "REVIEW",
            "PENDING",
            "ACCEPTED",
            "REJECTED",
            "REPAIR",
            "CLOSED"
        };

    public ObservableCollection<string> OperatorItems { get; } = new();

    public ObservableCollection<string> CustomerItems { get; } = new();

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText == value)
                return;

            _searchText = value;

            OnPropertyChanged();
            ApplyFilter();
        }
    }

    public string SelectedStatus
    {
        get => _selectedStatus;
        set
        {
            if (_selectedStatus == value)
                return;

            _selectedStatus = value;

            OnPropertyChanged();
            ApplyFilter();
        }
    }

    public string SelectedOperator
    {
        get => _selectedOperator;
        set
        {
            if (_selectedOperator == value)
                return;

            _selectedOperator = value;

            OnPropertyChanged();
            ApplyFilter();
        }
    }

    public string SelectedCustomer
    {
        get => _selectedCustomer;
        set
        {
            if (_selectedCustomer == value)
                return;

            _selectedCustomer = value;

            OnPropertyChanged();
            ApplyFilter();
        }
    }

    public JobHistoryRowModel? SelectedJob
    {
        get => _selectedJob;
        set
        {
            if (ReferenceEquals(_selectedJob, value))
                return;

            _selectedJob = value;

            OnPropertyChanged();
            OnPropertyChanged(nameof(HasSelectedJob));
            OnPropertyChanged(nameof(CanStartRT));
            OnPropertyChanged(nameof(CanStartReview));
            OnPropertyChanged(nameof(CanClose));
            OnPropertyChanged(nameof(CanReopen));
        }
    }

    public bool HasSelectedJob =>
        SelectedJob != null;

    public bool CanStartRT =>
        SelectedJob != null &&
        !IsClosedSelected();

    public bool CanStartReview =>
        SelectedJob != null &&
        !IsClosedSelected();

    public bool CanClose =>
        SelectedJob != null &&
        !IsClosedSelected();

    public bool CanReopen =>
        SelectedJob != null &&
        IsClosedSelected();

    public int TotalJobs =>
        FilteredJobs.Count;

    public ICommand RefreshCommand { get; }

    public ICommand ClearFilterCommand { get; }

    public ICommand OpenReviewCommand { get; }

    public ICommand StartRTCommand { get; }

    public ICommand StartReviewCommand { get; }

    public ICommand CloseWorkOrderCommand { get; }

    public ICommand ReopenWorkOrderCommand { get; }

    public JobHistoryViewModel()
    {
        _workOrderService = new WorkOrderService();
        _jobService = new JobService();
        _imageService = new ImageService();

        RefreshCommand =
            new RelayCommand(
                _ => LoadJobs());

        ClearFilterCommand =
            new RelayCommand(
                _ => ClearFilters());

        OpenReviewCommand =
            new RelayCommand(
                _ => OpenSelectedJob());

        StartRTCommand =
            new RelayCommand(
                _ => StartSelectedRT());

        StartReviewCommand =
            new RelayCommand(
                _ => StartSelectedReview());

        CloseWorkOrderCommand =
            new RelayCommand(
                _ => CloseSelectedWorkOrder());

        ReopenWorkOrderCommand =
            new RelayCommand(
                _ => ReopenSelectedWorkOrder());

        LoadJobs();
    }

    private void LoadJobs()
    {
        try
        {
            var workOrders =
                _workOrderService
                    .GetAll()
                    .OrderByDescending(
                        x => x.CreatedOn)
                    .ToList();

            Jobs.Clear();

            foreach (var workOrder in workOrders)
            {
                var row =
                    new JobHistoryRowModel
                    {
                        JobId = workOrder.Id,
                        JobNumber = workOrder.WorkOrderNumber,
                        Customer = workOrder.Customer,
                        Project = workOrder.Project,
                        Component = workOrder.Component,
                        Operator = workOrder.AssignedOperator,
                        Procedure = workOrder.Procedure,
                        Material = workOrder.Material,
                        Remark = workOrder.Remark,
                        CreatedOn = workOrder.CreatedOn,
                        IsClosed = workOrder.IsClosed,

                        WorkOrderStatus =
                            workOrder.Status,

                        WorkOrderResult =
                            workOrder.Result,

                        TotalPipes =
                            workOrder.TotalPipes,

                        CompletedShots =
                            workOrder.CompletedShots
                    };

                try
                {
                    var legacyJob =
                        _jobService.Get(
                            workOrder.Id);

                    row.WeldNumber =
                        legacyJob?.WeldNumber
                        ?? string.Empty;
                }
                catch
                {
                    row.WeldNumber =
                        string.Empty;
                }

                try
                {
                    var images =
                        _imageService
                            .GetByJob(workOrder.Id);

                    row.TotalShots =
                        images.Count;

                    row.AcceptedShots =
                        images.Count(
                            x =>
                                string.Equals(
                                    x.ReviewStatus,
                                    "ACCEPTED",
                                    StringComparison.OrdinalIgnoreCase));

                    row.RejectedShots =
                        images.Count(
                            x =>
                                string.Equals(
                                    x.ReviewStatus,
                                    "REJECTED",
                                    StringComparison.OrdinalIgnoreCase));

                    row.RepairShots =
                        images.Count(
                            x =>
                                string.Equals(
                                    x.ReviewStatus,
                                    "REPAIR",
                                    StringComparison.OrdinalIgnoreCase));

                    row.PendingShots =
                        images.Count(
                            x =>
                                string.IsNullOrWhiteSpace(
                                    x.ReviewStatus) ||
                                string.Equals(
                                    x.ReviewStatus,
                                    "PENDING",
                                    StringComparison.OrdinalIgnoreCase));
                }
                catch
                {
                    row.TotalShots = 0;
                    row.AcceptedShots = 0;
                    row.RejectedShots = 0;
                    row.RepairShots = 0;
                    row.PendingShots = 0;
                }

                Jobs.Add(row);
            }

            BuildFilterLists();
            ApplyFilter();
        }
        catch
        {
            Jobs.Clear();
            FilteredJobs.Clear();

            BuildFilterLists();
            ApplyFilter();
        }
    }

    private void BuildFilterLists()
    {
        var previousOperator =
            SelectedOperator;

        var previousCustomer =
            SelectedCustomer;

        OperatorItems.Clear();
        OperatorItems.Add("ALL");

        foreach (
            var value in Jobs
                .Select(x => x.Operator)
                .Where(
                    x =>
                        !string.IsNullOrWhiteSpace(x))
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x))
        {
            OperatorItems.Add(value);
        }

        CustomerItems.Clear();
        CustomerItems.Add("ALL");

        foreach (
            var value in Jobs
                .Select(x => x.Customer)
                .Where(
                    x =>
                        !string.IsNullOrWhiteSpace(x))
                .Distinct(
                    StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x))
        {
            CustomerItems.Add(value);
        }

        _selectedOperator =
            OperatorItems.Contains(
                previousOperator)
                ? previousOperator
                : "ALL";

        _selectedCustomer =
            CustomerItems.Contains(
                previousCustomer)
                ? previousCustomer
                : "ALL";

        OnPropertyChanged(
            nameof(SelectedOperator));

        OnPropertyChanged(
            nameof(SelectedCustomer));
    }

    private void ApplyFilter()
    {
        var search =
            SearchText.Trim();

        var filtered =
            Jobs
                .Where(
                    job =>
                        SelectedStatus == "ALL" ||
                        string.Equals(
                            job.OverallStatus,
                            SelectedStatus,
                            StringComparison.OrdinalIgnoreCase))
                .Where(
                    job =>
                        SelectedOperator == "ALL" ||
                        string.Equals(
                            job.Operator,
                            SelectedOperator,
                            StringComparison.OrdinalIgnoreCase))
                .Where(
                    job =>
                        SelectedCustomer == "ALL" ||
                        string.Equals(
                            job.Customer,
                            SelectedCustomer,
                            StringComparison.OrdinalIgnoreCase))
                .Where(
                    job =>
                        string.IsNullOrWhiteSpace(search) ||
                        Contains(job.JobNumber, search) ||
                        Contains(job.Customer, search) ||
                        Contains(job.Project, search) ||
                        Contains(job.Component, search) ||
                        Contains(job.WeldNumber, search) ||
                        Contains(job.Operator, search) ||
                        Contains(job.Procedure, search) ||
                        Contains(job.Material, search) ||
                        Contains(job.Remark, search) ||
                        Contains(job.WorkOrderStatus, search) ||
                        Contains(job.WorkOrderResult, search))
                .ToList();

        FilteredJobs.Clear();

        foreach (var job in filtered)
        {
            FilteredJobs.Add(job);
        }

        OnPropertyChanged(
            nameof(TotalJobs));

        if (SelectedJob != null &&
            !FilteredJobs.Contains(SelectedJob))
        {
            SelectedJob = null;
        }

        OnPropertyChanged(
            nameof(CanStartRT));

        OnPropertyChanged(
            nameof(CanStartReview));

        OnPropertyChanged(
            nameof(CanClose));

        OnPropertyChanged(
            nameof(CanReopen));
    }

    private void ClearFilters()
    {
        _searchText = string.Empty;
        _selectedStatus = "ALL";
        _selectedOperator = "ALL";
        _selectedCustomer = "ALL";

        OnPropertyChanged(
            nameof(SearchText));

        OnPropertyChanged(
            nameof(SelectedStatus));

        OnPropertyChanged(
            nameof(SelectedOperator));

        OnPropertyChanged(
            nameof(SelectedCustomer));

        SelectedJob = null;

        ApplyFilter();
    }

    private void StartSelectedRT()
    {
        if (SelectedJob == null)
        {
            ShowSelectMessage();
            return;
        }

        try
        {
            var workOrder =
                _workOrderService.Get(
                    SelectedJob.JobId);

            if (workOrder == null)
            {
                ShowNotFoundMessage();
                return;
            }

            if (workOrder.IsClosed)
            {
                MessageBox.Show(
                    "This Work Order is closed. Reopen it before starting RT.",
                    "Work Order",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            _workOrderService.StartRT(
                workOrder.Id);

            CurrentJobService.Instance
                .SetCurrentWorkOrder(workOrder);

            MessageBox.Show(
                $"RT started for Work Order:\n\n{workOrder.WorkOrderNumber}",
                "RT Started",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            LoadJobs();
            SelectRowById(workOrder.Id);
        }
        catch (Exception ex)
        {
            ShowError(
                "Unable to start RT.",
                ex);
        }
    }

    private void StartSelectedReview()
    {
        if (SelectedJob == null)
        {
            ShowSelectMessage();
            return;
        }

        try
        {
            var workOrder =
                _workOrderService.Get(
                    SelectedJob.JobId);

            if (workOrder == null)
            {
                ShowNotFoundMessage();
                return;
            }

            if (workOrder.IsClosed)
            {
                MessageBox.Show(
                    "This Work Order is closed. Reopen it before starting Review.",
                    "Work Order",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            _workOrderService.StartReview(
                workOrder.Id);

            CurrentJobService.Instance
                .SetCurrentWorkOrder(workOrder);

            MessageBox.Show(
                $"Review started for Work Order:\n\n{workOrder.WorkOrderNumber}",
                "Review Started",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            LoadJobs();
            SelectRowById(workOrder.Id);
        }
        catch (Exception ex)
        {
            ShowError(
                "Unable to start Review.",
                ex);
        }
    }

    private void CloseSelectedWorkOrder()
    {
        if (SelectedJob == null)
        {
            ShowSelectMessage();
            return;
        }

        try
        {
            var workOrder =
                _workOrderService.Get(
                    SelectedJob.JobId);

            if (workOrder == null)
            {
                ShowNotFoundMessage();
                return;
            }

            if (workOrder.IsClosed)
            {
                MessageBox.Show(
                    "This Work Order is already closed.",
                    "Work Order",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            var result =
                MessageBox.Show(
                    $"Close Work Order?\n\n{workOrder.WorkOrderNumber}",
                    "Close Work Order",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            _workOrderService.Close(
                workOrder.Id);

            if (CurrentJobService.Instance
                .CurrentWorkOrder?.Id == workOrder.Id)
            {
                CurrentJobService.Instance.Clear();
            }

            LoadJobs();
            SelectRowById(workOrder.Id);
        }
        catch (Exception ex)
        {
            ShowError(
                "Unable to close Work Order.",
                ex);
        }
    }

    private void ReopenSelectedWorkOrder()
    {
        if (SelectedJob == null)
        {
            ShowSelectMessage();
            return;
        }

        try
        {
            var workOrder =
                _workOrderService.Get(
                    SelectedJob.JobId);

            if (workOrder == null)
            {
                ShowNotFoundMessage();
                return;
            }

            if (!workOrder.IsClosed)
            {
                MessageBox.Show(
                    "This Work Order is already open.",
                    "Work Order",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }

            var result =
                MessageBox.Show(
                    $"Reopen Work Order?\n\n{workOrder.WorkOrderNumber}",
                    "Reopen Work Order",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            _workOrderService.Reopen(
                workOrder.Id);

            LoadJobs();
            SelectRowById(workOrder.Id);
        }
        catch (Exception ex)
        {
            ShowError(
                "Unable to reopen Work Order.",
                ex);
        }
    }

    private void OpenSelectedJob()
    {
        if (SelectedJob == null)
        {
            ShowSelectMessage();
            return;
        }

        try
        {
            var workOrder =
                _workOrderService.Get(
                    SelectedJob.JobId);

            if (workOrder == null)
            {
                ShowNotFoundMessage();
                return;
            }

            CurrentJobService.Instance
                .SetCurrentWorkOrder(workOrder);

            var mainWindow =
                Application.Current?.MainWindow;

            if (mainWindow == null)
            {
                MessageBox.Show(
                    "Main application window was not found.",
                    "Navigation Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            var reviewView =
                new ReviewView();

            if (reviewView.DataContext
                is ReviewViewModel reviewViewModel)
            {
                reviewViewModel.SelectedWorkOrder =
                    workOrder.WorkOrderNumber;
            }

            if (mainWindow.DataContext
                is ShellViewModel shellViewModel)
            {
                shellViewModel.CurrentPage =
                    reviewView;

                return;
            }

            MessageBox.Show(
                "Shell navigation context was not found.",
                "Navigation Error",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
        catch (Exception ex)
        {
            ShowError(
                "Unable to open Review.",
                ex);
        }
    }

    private void SelectRowById(Guid workOrderId)
    {
        var row =
            FilteredJobs.FirstOrDefault(
                x => x.JobId == workOrderId);

        SelectedJob = row;
    }

    private bool IsClosedSelected()
    {
        return
            SelectedJob != null &&
            SelectedJob.IsClosed;
    }

    private static void ShowSelectMessage()
    {
        MessageBox.Show(
            "Please select a Job / Work Order first.",
            "Job / Work Order",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private static void ShowNotFoundMessage()
    {
        MessageBox.Show(
            "Selected Work Order could not be found.",
            "Work Order",
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }

    private static void ShowError(
        string message,
        Exception ex)
    {
        MessageBox.Show(
            $"{message}\n\n{ex.Message}",
            "Work Order Error",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }

    private static bool Contains(
        string? source,
        string search)
    {
        return
            !string.IsNullOrWhiteSpace(source) &&
            source.Contains(
                search,
                StringComparison.OrdinalIgnoreCase);
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

