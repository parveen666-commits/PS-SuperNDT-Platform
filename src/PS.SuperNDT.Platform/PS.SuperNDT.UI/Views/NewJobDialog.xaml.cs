using System.Windows;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class NewJobDialog : Window
{
    private readonly JobDialogViewModel _viewModel;

    public NewJobDialog()
    {
        InitializeComponent();

        _viewModel = new JobDialogViewModel();
        DataContext = _viewModel;
    }

    private void CreateJob_Click(object sender, RoutedEventArgs e)
    {
        var job = new JobModel
        {
            JobNumber = _viewModel.JobNumber,
            Customer = _viewModel.Customer,
            Project = _viewModel.Project,
            Component = _viewModel.Component,
            WeldNumber = _viewModel.WeldNumber,
            Operator = _viewModel.Operator,
            Procedure = _viewModel.Procedure,
            Material = _viewModel.Material,
            Remark = _viewModel.Remarks,
            CreatedOn = System.DateTime.Now,
            IsClosed = false
        };

        CurrentJobService.Instance.SetCurrentJob(job);

        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}