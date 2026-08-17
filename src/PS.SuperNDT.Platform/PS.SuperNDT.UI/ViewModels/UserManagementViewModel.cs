using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using PS.SuperNDT.UI.Commands;
using PS.SuperNDT.UI.Dialogs;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public class UserManagementViewModel : INotifyPropertyChanged
{
    private readonly UserManagementService _userManagementService;
    private readonly AuditLogService _auditLogService;

    private UserModel? _selectedUser;

    public ObservableCollection<UserModel> Users { get; } =
        new();

    public UserModel? SelectedUser
    {
        get => _selectedUser;
        set
        {
            if (_selectedUser == value)
                return;

            _selectedUser = value;

            OnPropertyChanged();
            OnPropertyChanged(nameof(CanEditUser));
            OnPropertyChanged(nameof(CanDeleteUser));
        }
    }

    public bool CanEditUser =>
        SelectedUser != null;

    public bool CanDeleteUser =>
        SelectedUser != null &&
        !string.Equals(
            SelectedUser.Username,
            "admin",
            StringComparison.OrdinalIgnoreCase);

    public ICommand AddUserCommand { get; }

    public ICommand UpdateUserCommand { get; }

    public ICommand DeleteUserCommand { get; }

    public ICommand RefreshCommand { get; }

    public UserManagementViewModel()
    {
        _userManagementService =
            new UserManagementService();

        _auditLogService =
            new AuditLogService();

        AddUserCommand =
            new RelayCommand(
                _ => AddUser());

        UpdateUserCommand =
            new RelayCommand(
                _ => UpdateUser(),
                _ => CanEditUser);

        DeleteUserCommand =
            new RelayCommand(
                _ => DeleteUser(),
                _ => CanDeleteUser);

        RefreshCommand =
            new RelayCommand(
                _ => LoadUsers());

        LoadUsers();
    }

    public void LoadUsers()
    {
        Users.Clear();

        foreach (var user in _userManagementService.GetAll())
        {
            Users.Add(user);
        }

        SelectedUser = null;

        RaiseCommandCanExecuteChanged();
    }

    private void AddUser()
    {
        var user =
            new UserModel
            {
                Username =
                    $"user{Users.Count + 1}",

                FullName =
                    "New User",

                Role =
                    UserRole.Operator,

                IsActive =
                    true
            };

        var dialog =
            new UserEditorDialog(user)
            {
                Owner =
                    GetOwnerWindow()
            };

        if (dialog.ShowDialog() != true)
            return;

        if (!_userManagementService.AddOrUpdate(user))
        {
            MessageBox.Show(
                "Unable to create user. Username may already exist.",
                "User Management",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        _auditLogService.Add(
            UserSessionService.Instance.Username,
            "User Created",
            "User Management",
            $"User '{user.Username}' was created with role '{user.Role}'.");

        LoadUsers();
    }

    private void UpdateUser()
    {
        if (SelectedUser == null)
            return;

        var user =
            SelectedUser;

        var usernameBeforeEdit =
            user.Username;

        var dialog =
            new UserEditorDialog(user)
            {
                Owner =
                    GetOwnerWindow()
            };

        if (dialog.ShowDialog() != true)
            return;

        if (!_userManagementService.AddOrUpdate(user))
        {
            MessageBox.Show(
                "Unable to update user. Username may already exist.",
                "User Management",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        _auditLogService.Add(
            UserSessionService.Instance.Username,
            "User Updated",
            "User Management",
            $"User '{usernameBeforeEdit}' was updated.");

        LoadUsers();
    }

    private void DeleteUser()
    {
        if (!CanDeleteUser || SelectedUser == null)
            return;

        var username =
            SelectedUser.Username;

        var result =
            MessageBox.Show(
                $"Delete user '{username}'?",
                "User Management",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes)
            return;

        if (!_userManagementService.Delete(username))
        {
            MessageBox.Show(
                "Unable to delete user.",
                "User Management",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        _auditLogService.Add(
            UserSessionService.Instance.Username,
            "User Deleted",
            "User Management",
            $"User '{username}' was deleted.");

        LoadUsers();
    }

    private static Window? GetOwnerWindow()
    {
        return Application.Current?.Windows.Count > 0
            ? Application.Current.Windows[
                Application.Current.Windows.Count - 1]
            : null;
    }

    private void RaiseCommandCanExecuteChanged()
    {
        if (UpdateUserCommand is RelayCommand updateCommand)
        {
            updateCommand.RaiseCanExecuteChanged();
        }

        if (DeleteUserCommand is RelayCommand deleteCommand)
        {
            deleteCommand.RaiseCanExecuteChanged();
        }
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

        RaiseCommandCanExecuteChanged();
    }
}