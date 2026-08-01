using System.Windows.Controls;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class ReportSearchView : UserControl
{
    public ReportSearchView()
    {
        InitializeComponent();

        DataContext =
            new ReportSearchViewModel();
    }
}