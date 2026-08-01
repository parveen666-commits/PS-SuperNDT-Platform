using System.Windows.Controls;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class ReportAuditView : UserControl
{
    public ReportAuditView()
    {
        InitializeComponent();

        DataContext =
            new ReportAuditViewModel();
    }
}