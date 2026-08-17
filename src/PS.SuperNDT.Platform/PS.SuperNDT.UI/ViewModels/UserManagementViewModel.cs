using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PS.SuperNDT.UI.Commands;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public class UserManagementViewModel : INotifyPropertyChanged
{
    private readonly UserManagementService _userManagementService;

    private UserRoleModel? _selectedUser;

    public ObservableCollection<UserRoleModel> Users { get; } =
        new();

    public UserRoleModel? SelectedUser
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
        var user = new UserRoleModel
        {
            Username = $"user{Users.Count + 1}",
            FullName = "New User",
            Role = "Operator",
            IsActive = true
        };

        _userManagementService.AddOrUpdate(user);

        LoadUsers();
    }

    private void UpdateUser()
    {
        if (SelectedUser == null)
            return;

        _userManagementService.AddOrUpdate(
            SelectedUser);

        LoadUsers();
    }

    private void DeleteUser()
    {
        if (!CanDeleteUser || SelectedUser == null)
            return;

        _userManagementService.Delete(
            SelectedUser.Username);

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