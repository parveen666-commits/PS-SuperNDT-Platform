using System.Windows.Controls;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class ReportEditorView : UserControl
{
    public ReportEditorView()
    {
        InitializeComponent();

        DataContext = new ReportEditorViewModel();
    }
}