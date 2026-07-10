using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public class LoginViewModel : INotifyPropertyChanged
{
    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _message = string.Empty;

    public string Username
    {
        get => _username;
        set
        {
            if (_username == value)
                return;

            _username = value;
            OnPropertyChanged();
        }
    }

    public string Password
    {
        get => _password;
        set
        {
            if (_password == value)
                return;

            _password = value;
            OnPropertyChanged();
        }
    }

    public string Message
    {
        get => _message;
        set
        {
            if (_message == value)
                return;

            _message = value;
            OnPropertyChanged();
        }
    }

    public bool Login()
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            Message = "Enter Username";
            return false;
        }

        UserSessionService.Instance.Login(
            Username,
            Username,
            "Administrator");

        Message = "Login Successful";

        return true;
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