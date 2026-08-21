using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using PS.SuperNDT.UI.Commands;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;
using PS.SuperNDT.UI.Views;

namespace PS.SuperNDT.UI.ViewModels;

public class ShellViewModel : INotifyPropertyChanged
{
    private readonly AuthorizationService _authorizationService;

    public ObservableCollection<NavigationItem> MenuItems { get; }

    public ICommand NavigateCommand { get; }

    private UserControl _currentPage =
        new DashboardView();

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
        _authorizationService =
            new AuthorizationService(
                new AccessControlService(
                    new UserRoleService()));

        MenuItems =
            new ObservableCollection<NavigationItem>
            {
                new NavigationItem
                {
                    Title = "Dashboard"
                },

                new NavigationItem
                {
                    Title = "Acquisition"
                },

                new NavigationItem
                {
                    Title = "Job / Work Order History"
                },

                new NavigationItem
                {
                    Title = "Review"
                },

                new NavigationItem
                {
                    Title = "Calculator"
                },

                new NavigationItem
                {
                    Title = "Reports"
                },

                new NavigationItem
                {
                    Title = "Settings"
                }
            };

        if (_authorizationService.CanManageUsers())
        {
            MenuItems.Add(
                new NavigationItem
                {
                    Title = "User Management"
                });
        }

        if (_authorizationService.CanViewAuditLog())
        {
            MenuItems.Add(
                new NavigationItem
                {
                    Title = "Audit Log"
                });
        }

        NavigateCommand =
            new RelayCommand(
                item =>
                    Navigate(
                        item as NavigationItem));

        var timer =
            new DispatcherTimer
            {
                Interval =
                    TimeSpan.FromSeconds(1)
            };

        timer.Tick +=
            (s, e) =>
            {
                CurrentTime =
                    DateTime.Now.ToString(
                        "dd-MMM-yyyy HH:mm:ss");
            };

        timer.Start();
    }

    private void Navigate(
        NavigationItem? item)
    {
        if (item == null)
            return;

        switch (item.Title)
        {
            case "Dashboard":

                CurrentPage =
                    new DashboardView();

                break;

            case "Acquisition":

                if (_authorizationService.CanCreateJob())
                {
                    CurrentPage =
                        new AcquisitionView();
                }

                break;

            case "Job / Work Order History":

                if (_authorizationService.CanReview())
                {
                    CurrentPage =
                        new JobHistoryView();
                }

                break;

            case "Review":

                if (_authorizationService.CanReview())
                {
                    CurrentPage =
                        new ReviewView();
                }

                break;

            case "Calculator":

                CurrentPage =
                    new CalculatorView();

                break;

            case "Reports":

                if (_authorizationService.CanGenerateReports())
                {
                    CurrentPage =
                        new ReportDashboardView();
                }

                break;

            case "Settings":

                if (_authorizationService.CanOpenSettings())
                {
                    CurrentPage =
                        new SettingsView();
                }

                break;

            case "User Management":

                if (_authorizationService.CanManageUsers())
                {
                    CurrentPage =
                        new UserManagementView();
                }

                break;

            case "Audit Log":

                if (_authorizationService.CanViewAuditLog())
                {
                    CurrentPage =
                        new AuditLogView();
                }

                break;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(
        [CallerMemberName]
        string name = "")
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(
                name));
    }
}