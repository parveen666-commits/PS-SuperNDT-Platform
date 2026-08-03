using System.Windows;
using System.Windows.Controls;
using PS.SuperNDT.UI.ViewModels;

namespace PS.SuperNDT.UI.Views;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();

        DataContext =
            new LoginViewModel();

        PasswordBox.PasswordChanged +=
            PasswordBox_PasswordChanged;
    }


    private void PasswordBox_PasswordChanged(
        object sender,
        RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel viewModel)
        {
            viewModel.Password =
                PasswordBox.Password;
        }
    }


    private void LoginButton_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (DataContext is LoginViewModel viewModel)
        {
            bool result =
                viewModel.Login();


            if (result)
            {
                var window =
                    Window.GetWindow(this);


                if (window != null)
                {
                    window.DialogResult = true;
                }
            }
        }
    }
}