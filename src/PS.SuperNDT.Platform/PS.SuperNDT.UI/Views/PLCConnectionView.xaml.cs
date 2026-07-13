using System.Windows.Controls;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class PLCConnectionView : UserControl
{
    public PLCConnectionView()
    {
        InitializeComponent();

        DataContext = new PLCConnectionViewModel();
    }
}