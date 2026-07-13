using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class AcquisitionSessionViewModel : INotifyPropertyChanged
{
    private readonly AcquisitionSessionService _service = new();

    private AcquisitionSessionModel? _selectedSession;

    public ObservableCollection<AcquisitionSessionModel> Sessions { get; } = new();

    public AcquisitionSessionModel? SelectedSession
    {
        get => _selectedSession;
        set
        {
            _selectedSession = value;
            OnPropertyChanged();
        }
    }

    public AcquisitionSessionViewModel()
    {
        Load();
    }

    public void Load()
    {
        Sessions.Clear();

        foreach (var session in _service.GetAll())
        {
            Sessions.Add(session);
        }
    }

    public void AddNew()
    {
        var session = new AcquisitionSessionModel
        {
            SessionNumber = $"SES-{DateTime.Now:yyyyMMdd-HHmmss}",
            SessionStatus = "Ready",
            StartTime = DateTime.Now
        };

        _service.Save(session);

        Load();
    }

    public void StartSelected()
    {
        if (SelectedSession == null)
            return;

        _service.StartSession(SelectedSession.Id);

        Load();
    }

    public void RegisterAcceptedShot()
    {
        if (SelectedSession == null)
            return;

        _service.RegisterShot(SelectedSession.Id, true);

        Load();
    }

    public void RegisterRejectedShot()
    {
        if (SelectedSession == null)
            return;

        _service.RegisterShot(SelectedSession.Id, false);

        Load();
    }

    public void CompleteSelected()
    {
        if (SelectedSession == null)
            return;

        _service.CompleteSession(SelectedSession.Id);

        Load();
    }

    public void DeleteSelected()
    {
        if (SelectedSession == null)
            return;

        _service.Delete(SelectedSession.Id);

        Load();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}