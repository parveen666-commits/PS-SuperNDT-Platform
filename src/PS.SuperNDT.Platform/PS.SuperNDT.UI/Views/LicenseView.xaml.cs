using System.Windows.Controls;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class LicenseView : UserControl
{
    public LicenseView()
    {
        InitializeComponent();

        DataContext = new LicenseViewModel();
    }
}