using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
        }
    }

    public UserManagementViewModel()
    {
        _userManagementService =
            new UserManagementService();

        LoadUsers();
    }

    public void LoadUsers()
    {
        Users.Clear();

        foreach (var user in _userManagementService.GetAll())
        {
            Users.Add(user);
        }
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