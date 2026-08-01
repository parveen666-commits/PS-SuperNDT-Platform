using System;
using System.Collections.Generic;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportWorkflowService
{
    private readonly List<ReportDataModel> _reports = new();


    public void CreateReport(
        ReportDataModel report,
        string createdBy)
    {
        ArgumentNullException.ThrowIfNull(report);


        report.Id =
            report.Id == Guid.Empty
                ? Guid.NewGuid()
                : report.Id;


        report.GeneratedDate =
            DateTime.Now;


        _reports.Add(report);


        new ReportAuditService()
            .Record(
                report.Id,
                report.ReportNumber,
                "Create",
                "Report created",
                createdBy);
    }


    public void SubmitForApproval(
        ReportDataModel report,
        string submittedBy,
        string approvalLevel,
        string designation)
    {
        ArgumentNullException.ThrowIfNull(report);


        var approval =
            new ReportApprovalModel
            {
                ReportId = report.Id,

                ReportNumber =
                    report.ReportNumber,

                ApprovalLevel =
                    approvalLevel,

                Designation =
                    designation,

                SubmittedBy =
                    submittedBy,

                SubmittedOn =
                    DateTime.Now
            };


        new ReportApprovalService()
            .SubmitForApproval(
                approval);


        new ReportAuditService()
            .Record(
                report.Id,
                report.ReportNumber,
                "Submit Approval",
                "Report submitted for approval",
                submittedBy);
    }


    public IReadOnlyList<ReportDataModel> GetReports()
    {
        return _reports;
    }
}