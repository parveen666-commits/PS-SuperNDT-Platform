using System;
using System.Collections.ObjectModel;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportApprovalHistoryService
{
    private readonly ObservableCollection<ReportApprovalHistoryModel> _history;

    public ReportApprovalHistoryService()
    {
        _history = new ObservableCollection<ReportApprovalHistoryModel>();
    }

    public ReadOnlyObservableCollection<ReportApprovalHistoryModel> History =>
        new(_history);

    public ReportApprovalHistoryModel AddApproval(
        Guid reportId,
        string approvalStage,
        string approvedBy,
        string designation,
        string decision,
        string remarks)
    {
        var approval = new ReportApprovalHistoryModel
        {
            ReportId = reportId,
            ApprovalStage = approvalStage,
            ApprovedBy = approvedBy,
            Designation = designation,
            Decision = decision,
            Remarks = remarks,
            ApprovedOn = DateTime.Now
        };

        _history.Add(approval);

        return approval;
    }
}