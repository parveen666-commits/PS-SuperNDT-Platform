using System;
using System.Collections.Generic;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportWorkflowService
{
    private readonly ReportRepository _reportRepository;


    public ReportWorkflowService()
    {
        _reportRepository =
            new ReportRepository();
    }


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


        _reportRepository.Save(
            report);


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
                Id =
                    Guid.NewGuid(),

                ReportId =
                    report.Id,

                ReportNumber =
                    report.ReportNumber,

                ApprovalLevel =
                    approvalLevel,

                Designation =
                    designation,

                SubmittedBy =
                    submittedBy,

                SubmittedOn =
                    DateTime.Now,

                IsApproved =
                    false
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
        return _reportRepository.GetAll();
    }
}