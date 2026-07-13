using System.Windows.Controls;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class WeldView : UserControl
{
    public WeldView()
    {
        InitializeComponent();

        DataContext = new WeldViewModel();
    }
}