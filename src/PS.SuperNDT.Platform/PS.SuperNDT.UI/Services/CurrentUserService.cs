using System;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class CurrentUserService
{
    private static readonly Lazy<CurrentUserService> _instance =
        new(() => new CurrentUserService());

    public static CurrentUserService Instance =>
        _instance.Value;

    private CurrentUserService()
    {
    }

    public UserModel? CurrentUser { get; private set; }

    public bool IsLoggedIn =>
        CurrentUser != null;

    public event EventHandler? CurrentUserChanged;

    public void Login(UserModel user)
    {
        CurrentUser = user;

        CurrentUserChanged?.Invoke(
            this,
            EventArgs.Empty);
    }

    public void Logout()
    {
        CurrentUser = null;

        CurrentUserChanged?.Invoke(
            this,
            EventArgs.Empty);
    }

    public bool IsInRole(UserRole role)
    {
        return CurrentUser?.Role == role;
    }
}