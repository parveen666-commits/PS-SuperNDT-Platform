using System.Windows.Controls;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class AuditLogView : UserControl
{
    public AuditLogView()
    {
        InitializeComponent();
        DataContext = new AuditLogViewModel();
    }
}