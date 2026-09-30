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
    private readonly WorkOrderService _workOrderService = new();

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
            var jobId = Job?.Id ?? Guid.NewGuid();

            var workOrder =
                _workOrderService.Get(jobId)
                ?? new WorkOrderModel
                {
                    Id = jobId,
                    CreatedOn = DateTime.Now,
                    Status = "CREATED",
                    Result = "PENDING"
                };

            workOrder.WorkOrderNumber =
                _viewModel.JobNumber?.Trim() ?? string.Empty;

            workOrder.Customer =
                _viewModel.Customer?.Trim() ?? string.Empty;

            workOrder.Project =
                _viewModel.Project?.Trim() ?? string.Empty;

            workOrder.Component =
                _viewModel.Component?.Trim() ?? string.Empty;

            workOrder.AssignedOperator =
                _viewModel.Operator?.Trim() ?? string.Empty;

            workOrder.AssignedInspector =
                _viewModel.Inspector?.Trim() ?? string.Empty;

            workOrder.Procedure =
                _viewModel.Procedure?.Trim() ?? string.Empty;

            workOrder.Technique =
                _viewModel.Technique?.Trim() ?? string.Empty;

            workOrder.Material =
                _viewModel.Material?.Trim() ?? string.Empty;

            workOrder.MaterialSpecification =
                _viewModel.MaterialSpecification?.Trim() ?? string.Empty;

            workOrder.DrawingNumber =
                _viewModel.DrawingNumber?.Trim() ?? string.Empty;

            workOrder.PurchaseOrder =
                _viewModel.PurchaseOrder?.Trim() ?? string.Empty;

            workOrder.InspectionStandard =
                _viewModel.InspectionStandard?.Trim() ?? string.Empty;

            workOrder.AcceptanceStandard =
                _viewModel.AcceptanceStandard?.Trim() ?? string.Empty;

            workOrder.NominalThicknessMm =
                _viewModel.NominalThicknessMm;

            workOrder.PipeDiameterMm =
                _viewModel.PipeDiameterMm;

            workOrder.PipeLengthMm =
                _viewModel.PipeLengthMm;

            workOrder.DefaultShotSizeMm =
                _viewModel.DefaultShotSizeMm;

            workOrder.DefaultOverlapPercent =
                _viewModel.DefaultOverlapPercent;

            workOrder.Remark =
                _viewModel.Remarks?.Trim() ?? string.Empty;

            workOrder.UpdatedOn = DateTime.Now;

            _workOrderService.Save(workOrder);

            var job = Job ?? new JobModel
            {
                Id = jobId,
                CreatedOn = DateTime.Now,
                IsClosed = false
            };

            job.JobNumber =
                workOrder.WorkOrderNumber;

            job.Customer =
                workOrder.Customer;

            job.Project =
                workOrder.Project;

            job.Component =
                workOrder.Component;

            job.WeldNumber =
                _viewModel.WeldNumber?.Trim() ?? string.Empty;

            job.Operator =
                workOrder.AssignedOperator;

            job.Procedure =
                workOrder.Procedure;

            job.Material =
                workOrder.Material;

            job.Remark =
                workOrder.Remark;

            job.IsClosed =
                string.Equals(
                    workOrder.Status,
                    "CLOSED",
                    StringComparison.OrdinalIgnoreCase);

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
                $"Unable to save Work Order.\n\n{ex.Message}",
                "Work Order",
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