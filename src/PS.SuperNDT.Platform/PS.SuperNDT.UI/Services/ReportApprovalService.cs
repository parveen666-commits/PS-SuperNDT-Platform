using System;
using System.Collections.Generic;
using System.Linq;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportApprovalService
{
    private readonly List<ReportApprovalModel> _approvals = new();


    public IReadOnlyList<ReportApprovalModel> GetAll()
    {
        return _approvals;
    }


    public IEnumerable<ReportApprovalModel> GetByReport(
        Guid reportId)
    {
        return _approvals
            .Where(x => x.ReportId == reportId)
            .OrderByDescending(x => x.SubmittedOn);
    }


    public void SubmitForApproval(
        ReportApprovalModel approval)
    {
        ArgumentNullException.ThrowIfNull(approval);


        approval.Id =
            approval.Id == Guid.Empty
                ? Guid.NewGuid()
                : approval.Id;


        approval.SubmittedOn =
            DateTime.Now;


        approval.IsApproved = false;


        _approvals.Add(approval);
    }


    public void Approve(
        Guid approvalId,
        string approvedBy)
    {
        var approval =
            _approvals.FirstOrDefault(
                x => x.Id == approvalId);


        if (approval == null)
            return;


        approval.IsApproved = true;

        approval.ApprovedBy =
            approvedBy;

        approval.ApprovedOn =
            DateTime.Now;
    }


    public void Reject(
        Guid approvalId,
        string rejectedBy,
        string reason)
    {
        var approval =
            _approvals.FirstOrDefault(
                x => x.Id == approvalId);


        if (approval == null)
            return;


        approval.IsApproved = false;

        approval.ApprovedBy =
            rejectedBy;

        approval.Remarks =
            reason;
    }


    public void Clear()
    {
        _approvals.Clear();
    }
}