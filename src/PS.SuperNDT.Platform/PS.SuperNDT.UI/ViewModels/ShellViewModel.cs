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
    public ObservableCollection<NavigationItem> MenuItems { get; }

    public RelayCommand NavigateCommand { get; }

    private UserControl _currentPage = new DashboardView();

    public UserControl CurrentPage
    {
        get => _currentPage;
        set
        {
            _currentPage = value;
            OnPropertyChanged();
        }
    }

    private string _currentTime = "";

    public string CurrentTime
    {
        get => _currentTime;
        set
        {
            _currentTime = value;
            OnPropertyChanged();
        }
    }

    public ShellViewModel()
    {
        MenuItems = new ObservableCollection<NavigationItem>()
        {
            new NavigationItem()
            {
                Title = "Dashboard",
                ViewType = typeof(DashboardView)
            },

            new NavigationItem()
            {
                Title = "Acquisition",
                ViewType = typeof(AcquisitionView)
            },

            new NavigationItem()
            {
                Title = "Review",
                ViewType = typeof(ReviewView)
            },

            new NavigationItem()
            {
                Title = "Calculator",
                ViewType = typeof(CalculatorView)
            },

            new NavigationItem()
            {
                Title = "Reports",
                ViewType = typeof(ReportsView)
            },

            new NavigationItem()
            {
                Title = "Settings",
                ViewType = typeof(SettingsView)
            }
        };

        NavigateCommand = new RelayCommand(
            Navigate);

        DispatcherTimer timer = new DispatcherTimer();

        timer.Interval = TimeSpan.FromSeconds(1);

        timer.Tick += (s, e) =>
        {
            CurrentTime = DateTime.Now.ToString(
                "dd-MMM-yyyy HH:mm:ss");
        };

        timer.Start();
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