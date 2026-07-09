using System.Windows.Controls;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class AcquisitionView : UserControl
{
    public AcquisitionView()
    {
        InitializeComponent();

        DataContext = new AcquisitionViewModel();
    }
}