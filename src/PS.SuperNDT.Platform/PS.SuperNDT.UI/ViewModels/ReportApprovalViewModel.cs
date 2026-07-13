using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class ReportApprovalViewModel : INotifyPropertyChanged
{
    private ReportApprovalModel _approval;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ReportApprovalViewModel()
    {
        _approval = new ReportApprovalModel
        {
            ApprovalLevel = "Level-1 Review"
        };
    }

    public ReportApprovalModel Approval
    {
        get => _approval;
        set
        {
            _approval = value;
            OnPropertyChanged();
        }
    }

    public void Approve(
        string approvedBy,
        string designation,
        string remarks)
    {
        Approval.ApprovedBy = approvedBy;
        Approval.Designation = designation;
        Approval.Remarks = remarks;
        Approval.ApprovedOn = DateTime.Now;
        Approval.IsApproved = true;

        OnPropertyChanged(nameof(Approval));
    }

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}