using System.Windows.Controls;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class ReportsView : UserControl
{
    public ReportsView()
    {
        InitializeComponent();

        DataContext = new ReportsViewModel();
    }
}