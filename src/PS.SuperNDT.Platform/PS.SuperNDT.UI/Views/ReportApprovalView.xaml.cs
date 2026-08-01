using System.Windows;
using System.Windows.Controls;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class ReportApprovalView : UserControl
{
    public ReportApprovalView()
    {
        InitializeComponent();

        DataContext =
            new ReportApprovalViewModel();
    }


    private void Approve_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DataContext is ReportApprovalViewModel viewModel)
        {
            viewModel.Approve(
                "Current User");
        }
    }


    private void Reject_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DataContext is ReportApprovalViewModel viewModel)
        {
            viewModel.Reject(
                "Current User",
                "Rejected from approval screen");
        }
    }
}