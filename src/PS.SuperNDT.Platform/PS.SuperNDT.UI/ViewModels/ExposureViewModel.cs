using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ExposureViewModel : INotifyPropertyChanged
{
    private readonly ExposureService _exposureService = new();

    private ExposureModel? _selectedExposure;
    private Guid _currentWeldId;

    public ObservableCollection<ExposureModel> Exposures { get; } = new();

    public ExposureModel? SelectedExposure
    {
        get => _selectedExposure;
        set
        {
            _selectedExposure = value;
            OnPropertyChanged();
        }
    }

    public Guid CurrentWeldId
    {
        get => _currentWeldId;
        set
        {
            _currentWeldId = value;
            OnPropertyChanged();
        }
    }

    public void Load(Guid weldId)
    {
        CurrentWeldId = weldId;

        Exposures.Clear();

        foreach (var exposure in _exposureService.GetByWeld(weldId))
        {
            Exposures.Add(exposure);
        }
    }

    public void AddNew()
    {
        var exposure = new ExposureModel
        {
            WeldId = CurrentWeldId,
            ExposureNumber = $"EXP-{DateTime.Now:yyyyMMdd-HHmmss}",
            ExposureDateTime = DateTime.Now,
            Result = "Pending"
        };

        _exposureService.Save(exposure);

        Load(CurrentWeldId);
    }

    public void MarkAccepted()
    {
        if (SelectedExposure == null)
            return;

        _exposureService.Complete(SelectedExposure.Id, "Accepted");

        Load(CurrentWeldId);
    }

    public void MarkRejected()
    {
        if (SelectedExposure == null)
            return;

        _exposureService.Complete(SelectedExposure.Id, "Rejected");

        Load(CurrentWeldId);
    }

    public void DeleteSelected()
    {
        if (SelectedExposure == null)
            return;

        _exposureService.Delete(SelectedExposure.Id);

        Load(CurrentWeldId);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}