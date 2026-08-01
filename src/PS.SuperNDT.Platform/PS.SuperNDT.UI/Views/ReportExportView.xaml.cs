using System.Windows;
using System.Windows.Controls;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class ReportExportView : UserControl
{
    public ReportExportView()
    {
        InitializeComponent();

        DataContext =
            new ReportExportViewModel();
    }


    private void Export_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DataContext is ReportExportViewModel viewModel)
        {
            viewModel.Export();
        }
    }
}