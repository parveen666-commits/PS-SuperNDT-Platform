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

    private UserModel? _selectedUser;

    public ObservableCollection<UserModel> Users { get; } = new();

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
        var user = new UserModel
        {
            Username = string.Empty,
            Password = string.Empty,
            FullName = string.Empty,
            Role = UserRole.Operator,
            IsActive = true
        };

        var dialog =
            new UserEditorDialog(user)
            {
                Owner = Application.Current.MainWindow
            };

        if (dialog.ShowDialog() != true)
            return;

        if (!_userManagementService.AddOrUpdate(user))
        {
            MessageBox.Show(
                "Username already exists or the user data is invalid.",
                "User Management",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        LoadUsers();
    }

    private void UpdateUser()
    {
        if (SelectedUser == null)
            return;

        var originalUsername =
            SelectedUser.Username;

        var user =
            new UserModel
            {
                Id = SelectedUser.Id,
                Username = SelectedUser.Username,
                Password = SelectedUser.Password,
                FullName = SelectedUser.FullName,
                Role = SelectedUser.Role,
                IsActive = SelectedUser.IsActive,
                CreatedOn = SelectedUser.CreatedOn,
                LastLoginOn = SelectedUser.LastLoginOn
            };

        var dialog =
            new UserEditorDialog(user)
            {
                Owner = Application.Current.MainWindow
            };

        if (dialog.ShowDialog() != true)
            return;

        if (!_userManagementService.AddOrUpdate(user))
        {
            MessageBox.Show(
                "Username already exists or the user data is invalid.",
                "User Management",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        if (!string.Equals(
                originalUsername,
                user.Username,
                StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show(
                "Username updated successfully.",
                "User Management",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

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
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

        if (result != MessageBoxResult.Yes)
            return;

        if (!_userManagementService.Delete(username))
        {
            MessageBox.Show(
                "User could not be deleted.",
                "User Management",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        LoadUsers();
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
        [CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));

        RaiseCommandCanExecuteChanged();
    }
}