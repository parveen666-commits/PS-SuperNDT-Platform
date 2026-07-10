using System.Windows.Controls;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class UserManagementView : UserControl
{
    public UserManagementView()
    {
        InitializeComponent();
        DataContext = new UserManagementViewModel();
    }
}