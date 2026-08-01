using System.Windows.Controls;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class ReportHistoryView : UserControl
{
    public ReportHistoryView()
    {
        InitializeComponent();

        DataContext =
            new ReportHistoryViewModel();
    }
}