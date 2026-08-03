using System.Windows;
using System.Windows.Controls;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class ReportArchiveView : UserControl
{
    public ReportArchiveView()
    {
        InitializeComponent();

        DataContext =
            new ReportArchiveViewModel();
    }

    private void RemoveSelected_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DataContext is ReportArchiveViewModel viewModel)
        {
            viewModel.RemoveSelected();
        }
    }
}