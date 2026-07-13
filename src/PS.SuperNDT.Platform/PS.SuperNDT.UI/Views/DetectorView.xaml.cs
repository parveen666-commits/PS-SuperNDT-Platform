using System.Windows.Controls;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class DetectorView : UserControl
{
    public DetectorView()
    {
        InitializeComponent();

        DataContext = new DetectorViewModel();
    }
}