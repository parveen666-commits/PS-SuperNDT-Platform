using System;

namespace PS.SuperNDT.UI.Services;

public sealed class UserSessionService
{
    private static readonly Lazy<UserSessionService> _instance =
        new(() => new UserSessionService());

    public static UserSessionService Instance => _instance.Value;

    private UserSessionService()
    {
    }

    public string Username { get; private set; } = "Guest";

    public string FullName { get; private set; } = "Guest User";

    public string Role { get; private set; } = "Viewer";

    public bool IsLoggedIn { get; private set; }

    public DateTime LoginTime { get; private set; }

    public event EventHandler? SessionChanged;

    public void Login(
        string username,
        string fullName,
        string role)
    {
        Username = username;
        FullName = fullName;
        Role = role;

        IsLoggedIn = true;
        LoginTime = DateTime.Now;

        SessionChanged?.Invoke(
            this,
            EventArgs.Empty);
    }

    public void Logout()
    {
        Username = "Guest";
        FullName = "Guest User";
        Role = "Viewer";

        IsLoggedIn = false;

        SessionChanged?.Invoke(
            this,
            EventArgs.Empty);
    }
}