using System.Windows;
using System.Windows.Controls;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class ReportWorkflowView : UserControl
{
    public ReportWorkflowView()
    {
        InitializeComponent();

        DataContext =
            new ReportWorkflowViewModel();
    }


    private void CreateReport_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DataContext is ReportWorkflowViewModel viewModel)
        {
            viewModel.CreateReport(
                "Current User");
        }
    }


    private void SubmitApproval_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DataContext is ReportWorkflowViewModel viewModel)
        {
            viewModel.SubmitApproval(
                "Current User",
                "Level 2",
                "Inspector");
        }
    }
}