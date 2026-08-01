using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ReportApprovalViewModel : INotifyPropertyChanged
{
    private readonly ReportApprovalService _approvalService;

    public ObservableCollection<ReportApprovalModel> Approvals { get; } =
        new();


    private ReportApprovalModel? _selectedApproval;


    public ReportApprovalModel? SelectedApproval
    {
        get => _selectedApproval;
        set
        {
            if (_selectedApproval == value)
                return;

            _selectedApproval = value;
            OnPropertyChanged();
        }
    }


    private string _statusMessage = string.Empty;


    public string StatusMessage
    {
        get => _statusMessage;
        private set
        {
            if (_statusMessage == value)
                return;

            _statusMessage = value;
            OnPropertyChanged();
        }
    }


    public ReportApprovalViewModel()
    {
        _approvalService =
            new ReportApprovalService();

        Load();
    }


    private void Load()
    {
        Approvals.Clear();

        foreach (var item in _approvalService.GetAll())
        {
            Approvals.Add(item);
        }
    }


    public void Approve(
        string approvedBy)
    {
        if (SelectedApproval == null)
        {
            StatusMessage =
                "Select approval record.";

            return;
        }


        _approvalService.Approve(
            SelectedApproval.Id,
            approvedBy);


        StatusMessage =
            "Report approved successfully.";

        Load();
    }


    public void Reject(
        string rejectedBy,
        string reason)
    {
        if (SelectedApproval == null)
        {
            StatusMessage =
                "Select approval record.";

            return;
        }


        _approvalService.Reject(
            SelectedApproval.Id,
            rejectedBy,
            reason);


        StatusMessage =
            "Report rejected.";

        Load();
    }


    public event PropertyChangedEventHandler? PropertyChanged;


    private void OnPropertyChanged(
        [CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}