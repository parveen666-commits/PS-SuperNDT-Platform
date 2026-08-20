using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ShotPlanViewModel : INotifyPropertyChanged
{
    private readonly ShotPlanService _shotPlanService = new();

    private Guid _jobId;
    private string _pipeId = string.Empty;
    private string _weldNumber = string.Empty;
    private double _pipeLengthMm;
    private double _shotLengthMm = 300;
    private double _overlapMm = 10;
    private bool _rulerEnabled = true;
    private bool _pipeIdOverlayEnabled = true;
    private string _acquisitionMode = "Manual";
    private string _direction = "LeftToRight";
    private string _planStatus = "Ready";
    private int _currentShotNumber;
    private ShotPlanItemModel? _selectedShot;
    private ShotPlanModel? _currentPlan;

    public ShotPlanViewModel()
    {
        GenerateShotPlanCommand = new LocalCommand(
            GenerateShotPlan,
            CanGenerateShotPlan);

        ClearPlanCommand = new LocalCommand(
            ClearPlan);

        NextShotCommand = new LocalCommand(
            NextShot,
            CanMoveNext);

        PreviousShotCommand = new LocalCommand(
            PreviousShot,
            CanMovePrevious);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<ShotPlanItemModel> Shots { get; } = new();

    public Guid JobId
    {
        get => _jobId;
        set
        {
            if (_jobId == value)
                return;

            _jobId = value;

            OnPropertyChanged();
            RefreshCommands();
        }
    }

    public string PipeId
    {
        get => _pipeId;
        set
        {
            if (_pipeId == value)
                return;

            _pipeId = value;

            OnPropertyChanged();
            RefreshCommands();
        }
    }

    public string WeldNumber
    {
        get => _weldNumber;
        set
        {
            if (_weldNumber == value)
                return;

            _weldNumber = value;

            OnPropertyChanged();
        }
    }

    public double PipeLengthMm
    {
        get => _pipeLengthMm;
        set
        {
            if (Math.Abs(_pipeLengthMm - value) < 0.0001)
                return;

            _pipeLengthMm = value;

            OnPropertyChanged();
            OnPropertyChanged(nameof(TotalShots));
            OnPropertyChanged(nameof(PlanSummary));

            RefreshCommands();
        }
    }

    public double ShotLengthMm
    {
        get => _shotLengthMm;
        set
        {
            if (Math.Abs(_shotLengthMm - value) < 0.0001)
                return;

            _shotLengthMm = value;

            OnPropertyChanged();
            OnPropertyChanged(nameof(StepLengthMm));
            OnPropertyChanged(nameof(TotalShots));
            OnPropertyChanged(nameof(PlanSummary));

            RefreshCommands();
        }
    }

    public double OverlapMm
    {
        get => _overlapMm;
        set
        {
            if (Math.Abs(_overlapMm - value) < 0.0001)
                return;

            _overlapMm = value;

            OnPropertyChanged();
            OnPropertyChanged(nameof(StepLengthMm));
            OnPropertyChanged(nameof(TotalShots));
            OnPropertyChanged(nameof(PlanSummary));

            RefreshCommands();
        }
    }

    public double StepLengthMm =>
        ShotLengthMm > OverlapMm
            ? ShotLengthMm - OverlapMm
            : 0;

    public int TotalShots =>
        CalculateShotCount();

    public bool RulerEnabled
    {
        get => _rulerEnabled;
        set
        {
            if (_rulerEnabled == value)
                return;

            _rulerEnabled = value;

            OnPropertyChanged();
        }
    }

    public bool PipeIdOverlayEnabled
    {
        get => _pipeIdOverlayEnabled;
        set
        {
            if (_pipeIdOverlayEnabled == value)
                return;

            _pipeIdOverlayEnabled = value;

            OnPropertyChanged();
        }
    }

    public string AcquisitionMode
    {
        get => _acquisitionMode;
        set
        {
            if (_acquisitionMode == value)
                return;

            _acquisitionMode = string.IsNullOrWhiteSpace(value)
                ? "Manual"
                : value;

            OnPropertyChanged();
        }
    }

    public string Direction
    {
        get => _direction;
        set
        {
            if (_direction == value)
                return;

            _direction = string.IsNullOrWhiteSpace(value)
                ? "LeftToRight"
                : value;

            OnPropertyChanged();
        }
    }

    public string PlanStatus
    {
        get => _planStatus;
        private set
        {
            if (_planStatus == value)
                return;

            _planStatus = value;

            OnPropertyChanged();
        }
    }

    public int CurrentShotNumber
    {
        get => _currentShotNumber;
        private set
        {
            if (_currentShotNumber == value)
                return;

            _currentShotNumber = value;

            OnPropertyChanged();

            // Intentionally no CurrentPositionText notification.
            // Pipe position must not appear in the ruler area.

            RefreshCommands();
        }
    }

    public ShotPlanItemModel? SelectedShot
    {
        get => _selectedShot;
        set
        {
            if (ReferenceEquals(_selectedShot, value))
                return;

            _selectedShot = value;

            OnPropertyChanged();

            if (value != null)
            {
                CurrentShotNumber = value.ShotNumber;
            }

            RefreshCommands();
        }
    }

    public string PlanSummary
    {
        get
        {
            if (PipeLengthMm <= 0)
                return "Enter pipe length to calculate shot coverage.";

            if (ShotLengthMm <= 0)
                return "Enter a valid shot length.";

            if (OverlapMm < 0 ||
                OverlapMm >= ShotLengthMm)
            {
                return "Overlap must be smaller than shot length.";
            }

            return $"{PipeLengthMm:0.###} mm pipe  •  " +
                   $"{ShotLengthMm:0.###} mm shot  •  " +
                   $"{OverlapMm:0.###} mm overlap  •  " +
                   $"{StepLengthMm:0.###} mm step";
        }
    }

    public string ProgressText
    {
        get
        {
            if (Shots.Count == 0)
                return "0 / 0";

            int completed = Shots.Count(
                item => item.IsCaptured);

            return $"{completed} / {Shots.Count}";
        }
    }

    // Kept for compatibility with existing XAML.
    // It is intentionally blank so no Pipe Position box/text is shown.
    public string CurrentPositionText => string.Empty;

    public ICommand GenerateShotPlanCommand { get; }

    public ICommand ClearPlanCommand { get; }

    public ICommand NextShotCommand { get; }

    public ICommand PreviousShotCommand { get; }

    public void LoadForJob(JobModel? job)
    {
        if (job == null)
            return;

        JobId = job.Id;

        if (string.IsNullOrWhiteSpace(PipeId))
        {
            PipeId = job.JobNumber;
        }

        if (string.IsNullOrWhiteSpace(WeldNumber))
        {
            WeldNumber = job.WeldNumber;
        }

        OnPropertyChanged(nameof(PlanSummary));
        RefreshCommands();
    }

    private void GenerateShotPlan()
    {
        try
        {
            if (JobId == Guid.Empty)
            {
                PlanStatus = "No Job";
                return;
            }

            ShotPlanModel plan =
                _shotPlanService.CreatePlan(
                    JobId,
                    PipeId,
                    WeldNumber,
                    PipeLengthMm,
                    ShotLengthMm,
                    OverlapMm,
                    RulerEnabled,
                    PipeIdOverlayEnabled,
                    AcquisitionMode,
                    Direction);

            _currentPlan = plan;

            Shots.Clear();

            foreach (ShotPlanItemModel shot in plan.Shots)
            {
                Shots.Add(shot);
            }

            CurrentShotNumber =
                plan.CurrentShotNumber;

            SelectedShot =
                Shots.FirstOrDefault();

            PlanStatus =
                Shots.Count > 0
                    ? "Plan Ready"
                    : "Empty";

            RaisePlanProperties();
            RefreshCommands();
        }
        catch (ArgumentException)
        {
            PlanStatus = "Invalid Setup";
        }
        catch
        {
            PlanStatus = "Plan Error";
        }
    }

    private void ClearPlan()
    {
        Shots.Clear();

        _currentPlan = null;
        _selectedShot = null;

        CurrentShotNumber = 0;
        PlanStatus = "Ready";

        OnPropertyChanged(nameof(SelectedShot));
        OnPropertyChanged(nameof(ProgressText));

        RefreshCommands();
    }

    private void NextShot()
    {
        if (_currentPlan == null)
            return;

        if (!_currentPlan.MoveToNextPendingShot())
        {
            CurrentShotNumber =
                _currentPlan.CurrentShotNumber;

            PlanStatus =
                _currentPlan.IsCompleted
                    ? "Completed"
                    : "Plan Ready";

            RaisePlanProperties();
            RefreshCommands();

            return;
        }

        CurrentShotNumber =
            _currentPlan.CurrentShotNumber;

        SelectedShot =
            Shots.FirstOrDefault(
                shot => shot.ShotNumber == CurrentShotNumber);

        PlanStatus = "Acquisition";

        RaisePlanProperties();
        RefreshCommands();
    }

    private void PreviousShot()
    {
        if (Shots.Count == 0)
            return;

        ShotPlanItemModel? previous =
            Shots.Where(
                    item => item.ShotNumber < CurrentShotNumber)
                 .OrderByDescending(
                    item => item.ShotNumber)
                 .FirstOrDefault();

        if (previous == null)
            return;

        if (_currentPlan != null)
        {
            _currentPlan.MoveToShot(
                previous.ShotNumber);
        }

        CurrentShotNumber =
            previous.ShotNumber;

        SelectedShot = previous;

        PlanStatus = "Plan Ready";

        RaisePlanProperties();
        RefreshCommands();
    }

    private bool CanGenerateShotPlan()
    {
        return JobId != Guid.Empty
               && !string.IsNullOrWhiteSpace(PipeId)
               && PipeLengthMm > 0
               && ShotLengthMm > 0
               && OverlapMm >= 0
               && OverlapMm < ShotLengthMm;
    }

    private bool CanMoveNext()
    {
        return Shots.Count > 0
               && CurrentShotNumber > 0
               && CurrentShotNumber < Shots.Count;
    }

    private bool CanMovePrevious()
    {
        return Shots.Count > 0
               && CurrentShotNumber > 1;
    }

    private int CalculateShotCount()
    {
        if (PipeLengthMm <= 0 ||
            ShotLengthMm <= 0 ||
            OverlapMm < 0 ||
            OverlapMm >= ShotLengthMm)
        {
            return 0;
        }

        double step =
            ShotLengthMm - OverlapMm;

        if (PipeLengthMm <= ShotLengthMm)
            return 1;

        int count =
            (int)Math.Ceiling(
                (PipeLengthMm - ShotLengthMm) / step) + 1;

        return Math.Max(1, count);
    }

    private void RaisePlanProperties()
    {
        OnPropertyChanged(nameof(TotalShots));
        OnPropertyChanged(nameof(StepLengthMm));
        OnPropertyChanged(nameof(PlanSummary));
        OnPropertyChanged(nameof(ProgressText));
    }

    private void RefreshCommands()
    {
        if (GenerateShotPlanCommand is LocalCommand generate)
        {
            generate.RaiseCanExecuteChanged();
        }

        if (ClearPlanCommand is LocalCommand clear)
        {
            clear.RaiseCanExecuteChanged();
        }

        if (NextShotCommand is LocalCommand next)
        {
            next.RaiseCanExecuteChanged();
        }

        if (PreviousShotCommand is LocalCommand previous)
        {
            previous.RaiseCanExecuteChanged();
        }
    }

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }

    private sealed class LocalCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        public LocalCommand(
            Action execute,
            Func<bool>? canExecute = null)
        {
            _execute = execute ??
                       throw new ArgumentNullException(nameof(execute));

            _canExecute = canExecute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public void Execute(object? parameter)
        {
            _execute();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(
                this,
                EventArgs.Empty);
        }
    }
}