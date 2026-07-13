using System.Windows.Controls;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class ReportApprovalView : UserControl
{
    public ReportApprovalView()
    {
        InitializeComponent();

        DataContext = new ReportApprovalViewModel();
    }
}