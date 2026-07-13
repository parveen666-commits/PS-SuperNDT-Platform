using System.Windows.Controls;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class ExposureView : UserControl
{
    public ExposureView()
    {
        InitializeComponent();

        DataContext = new ExposureViewModel();
    }
}