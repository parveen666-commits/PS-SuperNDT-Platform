using System;
using System.Windows;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class NewJobDialog : Window
{
    private readonly JobDialogViewModel _viewModel;

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

    private void CreateJob_Click(object sender, RoutedEventArgs e)
    {
        var job = Job ?? new JobModel
        {
            Id = Guid.NewGuid(),
            CreatedOn = DateTime.Now,
            IsClosed = false
        };

        job.JobNumber = _viewModel.JobNumber;
        job.Customer = _viewModel.Customer;
        job.Project = _viewModel.Project;
        job.Component = _viewModel.Component;
        job.WeldNumber = _viewModel.WeldNumber;
        job.Operator = _viewModel.Operator;
        job.Procedure = _viewModel.Procedure;
        job.Material = _viewModel.Material;
        job.Remark = _viewModel.Remarks;

        CurrentJobService.Instance.SetCurrentJob(job);

        Job = job;

        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}