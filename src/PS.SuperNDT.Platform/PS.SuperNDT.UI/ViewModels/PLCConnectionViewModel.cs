using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class PLCConnectionViewModel : INotifyPropertyChanged
{
    private readonly PLCConnectionService _service = new();

    private PLCConnectionModel? _selectedPlc;

    public ObservableCollection<PLCConnectionModel> PlcConnections { get; } = new();

    public PLCConnectionModel? SelectedPlc
    {
        get => _selectedPlc;
        set
        {
            _selectedPlc = value;
            OnPropertyChanged();
        }
    }

    public PLCConnectionViewModel()
    {
        Load();
    }

    public void Load()
    {
        PlcConnections.Clear();

        foreach (var plc in _service.GetAll())
        {
            PlcConnections.Add(plc);
        }
    }

    public void AddDemoPlc()
    {
        var plc = new PLCConnectionModel
        {
            Name = "S7-200 Smart PLC",
            PlcType = "Siemens S7-200 Smart",
            IpAddress = "192.168.0.1",
            Rack = 0,
            Slot = 1,
            Port = 102,
            Status = "Offline"
        };

        _service.Save(plc);

        Load();
    }

    public void ConnectSelected()
    {
        if (SelectedPlc == null)
            return;

        _service.UpdateStatus(SelectedPlc.Id, true);

        Load();
    }

    public void DisconnectSelected()
    {
        if (SelectedPlc == null)
            return;

        _service.UpdateStatus(SelectedPlc.Id, false);

        Load();
    }

    public void DeleteSelected()
    {
        if (SelectedPlc == null)
            return;

        _service.Delete(SelectedPlc.Id);

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