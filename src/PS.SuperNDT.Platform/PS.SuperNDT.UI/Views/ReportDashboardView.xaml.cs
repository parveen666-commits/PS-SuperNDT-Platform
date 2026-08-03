using System.Windows.Controls;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class ReportDashboardView : UserControl
{
    public ReportDashboardView()
    {
        InitializeComponent();

        DataContext = new ReportDashboardViewModel();
    }
}