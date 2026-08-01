using System.Windows.Controls;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class ReportTemplateView : UserControl
{
    public ReportTemplateView()
    {
        InitializeComponent();

        DataContext =
            new ReportTemplateViewModel();
    }


    private void Template_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (DataContext is ReportTemplateViewModel viewModel)
        {
            viewModel.SelectTemplate();
        }
    }
}