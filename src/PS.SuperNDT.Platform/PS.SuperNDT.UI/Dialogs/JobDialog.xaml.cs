using System;
using System.Windows;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Dialogs;

public partial class JobDialog : Window
{
    private readonly JobDialogViewModel _viewModel;
    private readonly JobService _jobService;

    public JobDialog()
    {
        InitializeComponent();

        _viewModel = new JobDialogViewModel();
        _jobService = new JobService();

        DataContext = _viewModel;
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_viewModel.JobNumber))
        {
            MessageBox.Show(
                "Job Number is required.",
                "Validation",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        try
        {
            JobModel job = new()
            {
                Id = Guid.NewGuid(),
                JobNumber = _viewModel.JobNumber,
                Customer = _viewModel.Customer,
                Project = _viewModel.Project,
                Component = _viewModel.Component,
                WeldNumber = _viewModel.WeldNumber,
                Operator = _viewModel.Operator,
                Procedure = _viewModel.Procedure,
                Material = _viewModel.Material,
                Remark = _viewModel.Remarks,
                CreatedOn = DateTime.Now,
                IsClosed = false
            };

            _jobService.Save(job);

            // Set as active job for the whole application
            CurrentJobService.Instance.SetCurrentJob(job);

            MessageBox.Show(
                "Job saved successfully.",
                "PS SuperNDT",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                "Save Failed",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}