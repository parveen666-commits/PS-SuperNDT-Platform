using System.Windows.Controls;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class CustomerView : UserControl
{
    public CustomerView()
    {
        InitializeComponent();

        DataContext = new CustomerViewModel();
    }
}