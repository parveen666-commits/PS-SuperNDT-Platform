using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PS.SuperNDT.UI.Database;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportApprovalService
{
    public IReadOnlyList<ReportApprovalModel> GetAll()
    {
        using var db =
            new SuperNDTDbContext();

        return db.Set<ReportApprovalModel>()
                 .OrderByDescending(x => x.SubmittedOn)
                 .ToList();
    }


    public IEnumerable<ReportApprovalModel> GetByReport(
        Guid reportId)
    {
        using var db =
            new SuperNDTDbContext();

        return db.Set<ReportApprovalModel>()
                 .Where(x => x.ReportId == reportId)
                 .OrderByDescending(x => x.SubmittedOn)
                 .ToList();
    }


    public void SubmitForApproval(
        ReportApprovalModel approval)
    {
        ArgumentNullException.ThrowIfNull(approval);


        using var db =
            new SuperNDTDbContext();


        approval.Id =
            approval.Id == Guid.Empty
                ? Guid.NewGuid()
                : approval.Id;


        approval.SubmittedOn =
            DateTime.Now;


        approval.IsApproved =
            false;


        db.Set<ReportApprovalModel>()
          .Add(approval);


        db.SaveChanges();
    }


    public void Approve(
        Guid approvalId,
        string approvedBy)
    {
        using var db =
            new SuperNDTDbContext();


        var approval =
            db.Set<ReportApprovalModel>()
              .FirstOrDefault(
                  x => x.Id == approvalId);


        if (approval == null)
            return;


        approval.IsApproved =
            true;


        approval.ApprovedBy =
            approvedBy;


        approval.ApprovedOn =
            DateTime.Now;


        db.SaveChanges();
    }


    public void Reject(
        Guid approvalId,
        string rejectedBy,
        string reason)
    {
        using var db =
            new SuperNDTDbContext();


        var approval =
            db.Set<ReportApprovalModel>()
              .FirstOrDefault(
                  x => x.Id == approvalId);


        if (approval == null)
            return;


        approval.IsApproved =
            false;


        approval.ApprovedBy =
            rejectedBy;


        approval.Remarks =
            reason;


        db.SaveChanges();
    }


    public void Clear()
    {
        using var db =
            new SuperNDTDbContext();


        var approvals =
            db.Set<ReportApprovalModel>();


        db.RemoveRange(
            approvals);


        db.SaveChanges();
    }
}