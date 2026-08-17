using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Commands;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public class LoginViewModel : INotifyPropertyChanged
{
    private readonly UserService _userService;
    private readonly AuditLogService _auditLogService;

    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _message = string.Empty;

    public LoginViewModel()
    {
        _userService =
            new UserService();

        _auditLogService =
            new AuditLogService();

        LoginCommand =
            new RelayCommand(
                _ => Login());
    }

    public RelayCommand LoginCommand
    {
        get;
    }

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

        if (string.IsNullOrWhiteSpace(Password))
        {
            Message = "Enter Password";
            return false;
        }

        var user =
            _userService.Login(
                Username,
                Password);

        if (user == null)
        {
            _auditLogService.Add(
                Username.Trim(),
                "Login Failed",
                "Security",
                "Invalid username or password.");

            Message =
                "Invalid username or password";

            return false;
        }

        UserSessionService.Instance.Login(
            user.Username,
            user.FullName,
            user.Role.ToString());

        _auditLogService.Add(
            user.Username,
            "Login",
            "Security",
            $"User logged in successfully with role {user.Role}.");

        Message =
            "Login Successful";

        return true;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(
        [CallerMemberName]
        string propertyName = "")
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(
                propertyName));
    }
}