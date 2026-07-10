using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Threading;
using PS.SuperNDT.UI.Commands;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Views;

namespace PS.SuperNDT.UI.ViewModels;

public class ShellViewModel : INotifyPropertyChanged
{
    private readonly DispatcherTimer _timer;

    public ObservableCollection<NavigationItem> MenuItems { get; }

    public RelayCommand NavigateCommand { get; }

    private UserControl _currentPage = new DashboardView();

    public UserControl CurrentPage
    {
        get => _currentPage;
        set
        {
            if (_currentPage == value)
                return;

            _currentPage = value;
            OnPropertyChanged();
        }
    }

    private string _currentTime = string.Empty;

    public string CurrentTime
    {
        get => _currentTime;
        set
        {
            if (_currentTime == value)
                return;

            _currentTime = value;
            OnPropertyChanged();
        }
    }

    public ShellViewModel()
    {
        MenuItems = new ObservableCollection<NavigationItem>
        {
            new()
            {
                Title = "Dashboard",
                ViewType = typeof(DashboardView)
            },

            new()
            {
                Title = "Acquisition",
                ViewType = typeof(AcquisitionView)
            },

            new()
            {
                Title = "Review",
                ViewType = typeof(ReviewView)
            },

            new()
            {
                Title = "Calculator",
                ViewType = typeof(CalculatorView)
            },

            new()
            {
                Title = "Reports",
                ViewType = typeof(ReportsView)
            },

           new()
{
    Title = "User Management",
    ViewType = typeof(UserManagementView)
},

new()
{
    Title = "Audit Log",
    ViewType = typeof(AuditLogView)
},

new()
{
    Title = "Settings",
    ViewType = typeof(SettingsView)
}
        };

        NavigateCommand = new RelayCommand(Navigate);

        CurrentTime = DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss");

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        _timer.Tick += OnTimerTick;
        _timer.Start();
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        CurrentTime = DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss");
    }

    private void Navigate(object? parameter)
    {
        if (parameter is not NavigationItem item)
            return;

        if (item.ViewType == null)
            return;

        if (Activator.CreateInstance(item.ViewType) is UserControl view)
        {
            CurrentPage = view;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(
        [CallerMemberName] string name = "")
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(name));
    }
}