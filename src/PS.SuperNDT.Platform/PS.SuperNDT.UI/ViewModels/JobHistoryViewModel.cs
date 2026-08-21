using System;
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
            "CLOSED",
            "PENDING",
            "ACCEPTED",
            "REJECTED",
            "REPAIR"
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
        }
    }

    public bool HasSelectedJob =>
        SelectedJob != null;

    public int TotalJobs =>
        FilteredJobs.Count;

    public ICommand RefreshCommand { get; }

    public ICommand ClearFilterCommand { get; }

    public ICommand OpenReviewCommand { get; }

    public JobHistoryViewModel()
    {
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

        LoadJobs();
    }

    private void LoadJobs()
    {
        try
        {
            var jobs =
                _jobService
                    .GetAll()
                    .OrderByDescending(
                        x => x.CreatedOn)
                    .ToList();

            Jobs.Clear();

            foreach (var job in jobs)
            {
                var row =
                    new JobHistoryRowModel
                    {
                        JobId = job.Id,
                        JobNumber = job.JobNumber,
                        Customer = job.Customer,
                        Project = job.Project,
                        Component = job.Component,
                        WeldNumber = job.WeldNumber,
                        Operator = job.Operator,
                        Procedure = job.Procedure,
                        Material = job.Material,
                        Remark = job.Remark,
                        CreatedOn = job.CreatedOn,
                        IsClosed = job.IsClosed
                    };

                try
                {
                    var images =
                        _imageService
                            .GetByJob(job.Id);

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
                        Contains(job.Remark, search))
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

    private void OpenSelectedJob()
    {
        if (SelectedJob == null)
        {
            MessageBox.Show(
                "Please select a Job / Work Order first.",
                "Job / Work Order",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            return;
        }

        try
        {
            var job =
                _jobService.Get(
                    SelectedJob.JobId);

            if (job == null)
            {
                MessageBox.Show(
                    "Selected Job / Work Order could not be found.",
                    "Job / Work Order",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            /*
             * IMPORTANT:
             * Selected Job becomes the Current Job
             * before ReviewView is created.
             */
            CurrentJobService.Instance.SetCurrentJob(
                job);

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

            /*
             * Create Review first.
             * Then explicitly tell its ViewModel
             * which Work Order was selected.
             */
            var reviewView =
                new ReviewView();

            if (reviewView.DataContext
                is ReviewViewModel reviewViewModel)
            {
                reviewViewModel.SelectedWorkOrder =
                    job.JobNumber;
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
            MessageBox.Show(
                $"Unable to open Review.\n\n{ex.Message}",
                "Review Navigation Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
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