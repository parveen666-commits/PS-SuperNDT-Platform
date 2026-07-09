using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using PS.SuperNDT.UI.Commands;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class LoginViewModel : INotifyPropertyChanged
{
    private readonly UserService _userService = new();

    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _statusMessage = string.Empty;

    public RelayCommand LoginCommand { get; }

    public string Username
    {
        get => _username;
        set
        {
            _username = value;
            OnPropertyChanged();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged();
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            _statusMessage = value;
            OnPropertyChanged();
        }
    }

    public LoginViewModel()
    {
        LoginCommand =
            new RelayCommand(_ => Login());
    }

    private void Login()
    {
        var user =
            _userService.Login(
                Username,
                Password);

        if (user == null)
        {
            StatusMessage = "Invalid username or password.";

            MessageBox.Show(
                "Invalid username or password.",
                "PS SuperNDT",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        CurrentUserService.Instance.Login(user);

        StatusMessage =
            $"Welcome {user.FullName}";

        MessageBox.Show(
            $"Welcome {user.FullName}",
            "PS SuperNDT",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(
        [CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}