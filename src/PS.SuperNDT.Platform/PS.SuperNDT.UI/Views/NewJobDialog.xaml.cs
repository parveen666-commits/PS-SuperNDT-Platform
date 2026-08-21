using System;
using System.Windows;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class NewJobDialog : Window
{
    private readonly JobDialogViewModel _viewModel;

    private readonly JobService _jobService = new();

    public JobModel? Job { get; private set; }

    public NewJobDialog()
    {
        InitializeComponent();

        _viewModel = new JobDialogViewModel();
        DataContext = _viewModel;
    }

    public NewJobDialog(JobModel existingJob)
    {
        InitializeComponent();

        _viewModel = new JobDialogViewModel
        {
            JobNumber = existingJob.JobNumber,
            Customer = existingJob.Customer,
            Project = existingJob.Project,
            Component = existingJob.Component,
            WeldNumber = existingJob.WeldNumber,
            Operator = existingJob.Operator,
            Procedure = existingJob.Procedure,
            Material = existingJob.Material,
            Remarks = existingJob.Remark
        };

        Job = existingJob;
        DataContext = _viewModel;
    }

    private void CreateJob_Click(
        object sender,
        RoutedEventArgs e)
    {
        try
        {
            var job = Job ?? new JobModel
            {
                Id = Guid.NewGuid(),
                CreatedOn = DateTime.Now,
                IsClosed = false
            };

            job.JobNumber =
                _viewModel.JobNumber?.Trim() ?? string.Empty;

            job.Customer =
                _viewModel.Customer?.Trim() ?? string.Empty;

            job.Project =
                _viewModel.Project?.Trim() ?? string.Empty;

            job.Component =
                _viewModel.Component?.Trim() ?? string.Empty;

            job.WeldNumber =
                _viewModel.WeldNumber?.Trim() ?? string.Empty;

            job.Operator =
                _viewModel.Operator?.Trim() ?? string.Empty;

            job.Procedure =
                _viewModel.Procedure?.Trim() ?? string.Empty;

            job.Material =
                _viewModel.Material?.Trim() ?? string.Empty;

            job.Remark =
                _viewModel.Remarks?.Trim() ?? string.Empty;

            // IMPORTANT:
            // Persist the job before making it the current job.
            _jobService.Save(job);

            CurrentJobService.Instance.SetCurrentJob(job);

            Job = job;

            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                this,
                $"Unable to save job.\n\n{ex.Message}",
                "New Job",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void Cancel_Click(
        object sender,
        RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}