using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using PS.SuperNDT.UI.Commands;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Views;

namespace PS.SuperNDT.UI.ViewModels;

public class ShellViewModel : INotifyPropertyChanged
{
    public ObservableCollection<NavigationItem> MenuItems { get; }

    public ICommand NavigateCommand { get; }

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
            new NavigationItem(){Title="Dashboard"},
            new NavigationItem(){Title="Acquisition"},
            new NavigationItem(){Title="Review"},
            new NavigationItem(){Title="Calculator"},
            new NavigationItem(){Title="Reports"},
            new NavigationItem(){Title="Settings"}
        };

        NavigateCommand = new RelayCommand(
            item => Navigate(item as NavigationItem));

        DispatcherTimer timer = new DispatcherTimer();

        timer.Interval = TimeSpan.FromSeconds(1);

        timer.Tick += (s, e) =>
        {
            CurrentTime = DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss");
        };

        timer.Start();
    }

    private void Navigate(NavigationItem? item)
    {
        if (item == null)
            return;

        switch (item.Title)
        {
            case "Dashboard":
                CurrentPage = new DashboardView();
                break;

            case "Acquisition":
                CurrentPage = new AcquisitionView();
                break;

            case "Calculator":
                CurrentPage = new CalculatorView();
                break;

            case "Reports":
                CurrentPage = new ReportView();
                break;

            case "Review":
                CurrentPage = new ReviewView();
                break;

            case "Settings":
                CurrentPage = new SettingsView();
                break;
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