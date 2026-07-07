using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace PS.SuperNDT.UI.ViewModels;

public class DashboardViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public string DetectorStatus { get; set; } = "Connected";

    public string PLCStatus { get; set; } = "Offline";

    public string CurrentRecipe { get; set; } = "12\" TP304L";

    public string CurrentJob { get; set; } = "JOB-2026-001";

    public int TotalShots { get; set; } = 145;

    public int RejectCount { get; set; } = 3;

    public double StoragePercent { get; set; } = 68;

    private string _currentTime = "";

    public string CurrentTime
    {
        get => _currentTime;
        set
        {
            _currentTime = value;
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(nameof(CurrentTime)));
        }
    }

    public DashboardViewModel()
    {
        var timer = new DispatcherTimer();

        timer.Interval = TimeSpan.FromSeconds(1);

        timer.Tick += (_, _) =>
        {
            CurrentTime = DateTime.Now.ToString("dd-MMM-yyyy HH:mm:ss");
        };

        timer.Start();
    }
}