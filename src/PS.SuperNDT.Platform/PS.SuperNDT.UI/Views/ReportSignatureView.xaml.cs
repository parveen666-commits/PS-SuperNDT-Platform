using System.Windows;
using System.Windows.Controls;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class ReportSignatureView : UserControl
{
    public ReportSignatureView()
    {
        InitializeComponent();

        DataContext =
            new ReportSignatureViewModel();
    }


    private void VerifySignature_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DataContext is ReportSignatureViewModel viewModel)
        {
            viewModel.VerifySignature();
        }
    }
}