using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class WeldViewModel : INotifyPropertyChanged
{
    private readonly WeldService _weldService = new();

    private WeldModel? _selectedWeld;
    private Guid _currentJobId;

    public ObservableCollection<WeldModel> Welds { get; } = new();

    public WeldModel? SelectedWeld
    {
        get => _selectedWeld;
        set
        {
            _selectedWeld = value;
            OnPropertyChanged();
        }
    }

    public Guid CurrentJobId
    {
        get => _currentJobId;
        set
        {
            _currentJobId = value;
            OnPropertyChanged();
        }
    }

    public void Load(Guid jobId)
    {
        CurrentJobId = jobId;

        Welds.Clear();

        foreach (var weld in _weldService.GetByJob(jobId))
        {
            Welds.Add(weld);
        }
    }

    public void AddNew()
    {
        var weld = new WeldModel
        {
            JobId = CurrentJobId,
            WeldNumber = $"W-{DateTime.Now:HHmmss}",
            InspectionStatus = "Pending"
        };

        _weldService.Save(weld);

        Load(CurrentJobId);
    }

    public void DeleteSelected()
    {
        if (SelectedWeld == null)
            return;

        _weldService.Delete(SelectedWeld.Id);

        Load(CurrentJobId);
    }

    public void MarkAccepted()
    {
        if (SelectedWeld == null)
            return;

        _weldService.UpdateShotResult(SelectedWeld.Id, true);

        Load(CurrentJobId);
    }

    public void MarkRejected()
    {
        if (SelectedWeld == null)
            return;

        _weldService.UpdateShotResult(SelectedWeld.Id, false);

        Load(CurrentJobId);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}