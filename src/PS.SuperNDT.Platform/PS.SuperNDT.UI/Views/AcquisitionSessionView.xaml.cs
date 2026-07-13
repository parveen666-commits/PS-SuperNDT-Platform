using System.Windows.Controls;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class AcquisitionSessionView : UserControl
{
    public AcquisitionSessionView()
    {
        InitializeComponent();

        DataContext = new AcquisitionSessionViewModel();
    }
}