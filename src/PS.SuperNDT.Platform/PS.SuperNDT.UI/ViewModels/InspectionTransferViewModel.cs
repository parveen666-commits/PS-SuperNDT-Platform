using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using PS.SuperNDT.UI.Commands;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class InspectionTransferViewModel : INotifyPropertyChanged
{
    private readonly InspectionTransferService _transferService;
    private readonly TransferHistoryService _historyService;

    private bool _autoTransferEnabled;
    private string _destinationPath = string.Empty;

    public InspectionTransferViewModel()
    {
        _transferService = new InspectionTransferService();
        _historyService = new TransferHistoryService();

        ManualSendCommand = new RelayCommand(_ => ManualSend());
        ToggleAutoTransferCommand = new RelayCommand(_ => ToggleAutoTransfer());

        DestinationPath = @"C:\PS-SuperNDT\Transfer";

        LoadDemoQueue();
    }

    public ObservableCollection<InspectionTransferModel> Queue =>
        _transferService.Queue;

    public ObservableCollection<TransferHistoryModel> TransferHistory =>
        _historyService.History;

    public ICommand ManualSendCommand { get; }

    public ICommand ToggleAutoTransferCommand { get; }

    public bool AutoTransferEnabled
    {
        get => _autoTransferEnabled;
        set
        {
            _autoTransferEnabled = value;
            OnPropertyChanged();
        }
    }

    public string DestinationPath
    {
        get => _destinationPath;
        set
        {
            _destinationPath = value;
            OnPropertyChanged();
        }
    }

    private void ToggleAutoTransfer()
    {
        AutoTransferEnabled = !AutoTransferEnabled;
    }

    private void ManualSend()
    {
        foreach (var item in Queue)
        {
            if (item.Status != TransferStatus.Pending)
                continue;

            _transferService.MarkSending(item.Id);
            _transferService.MarkSent(item.Id);

            _historyService.Add(item, "Manual Transfer");
        }

        OnPropertyChanged(nameof(Queue));
        OnPropertyChanged(nameof(TransferHistory));
    }

    private void LoadDemoQueue()
    {
        _transferService.AddToQueue(
            "JOB-20260713-001",
            "INS-0001",
            @"C:\Images\Shot001.tif",
            DestinationPath,
            false,
            Environment.UserName);

        _transferService.AddToQueue(
            "JOB-20260713-001",
            "INS-0002",
            @"C:\Images\Shot002.tif",
            DestinationPath,
            false,
            Environment.UserName);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}