using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Threading;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Views;

namespace PS.SuperNDT.UI.ViewModels
{
    public class ShellViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<NavigationItem> MenuItems { get; }

        private UserControl _currentPage;
        public UserControl CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged();
            }
        }

        private string _currentTime;
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

            CurrentPage = new DashboardView();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) =>
            {
                CurrentTime = DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss");
            };
            timer.Start();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}