using System.Windows;
using System.Windows.Controls;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class LoginView : UserControl
{
    private readonly LoginViewModel _viewModel;

    public LoginView()
    {
        InitializeComponent();

        _viewModel = new LoginViewModel();

        DataContext = _viewModel;

        Loaded += LoginView_Loaded;
    }

    private void LoginView_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        PasswordBox.PasswordChanged +=
            PasswordBox_PasswordChanged;
    }

    private void PasswordBox_PasswordChanged(
        object sender,
        RoutedEventArgs e)
    {
        _viewModel.Password =
            PasswordBox.Password;
    }
}