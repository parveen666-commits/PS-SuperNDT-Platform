using System;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportWorkflowService
{
    private readonly ReportRepository _reportRepository;
    private readonly AuditLogService _auditLogService;
    private readonly ReportService _reportService;

    public ReportWorkflowService(
        ReportRepository reportRepository,
        AuditLogService auditLogService,
        ReportService reportService)
    {
        _reportRepository = reportRepository;
        _auditLogService = auditLogService;
        _reportService = reportService;
    }

    public ReportModel CreateReport(
        JobModel job,
        string inspector)
    {
        var report = new ReportModel
        {
            ReportNumber = $"RPT-{DateTime.Now:yyyyMMdd-HHmmss}",
            JobNumber = job.JobNumber,
            Customer = job.Customer,
            Project = job.Project,
            Component = job.Component,
            WeldNumber = job.WeldNumber,
            Inspector = inspector,
            ReportDate = DateTime.Now,
            Remarks = job.Remark
        };

        report.ReportFilePath =
            _reportService.GenerateJobReport(job);

        _reportRepository.AddOrUpdate(report);

        _auditLogService.Add(
            inspector,
            "REPORT",
            "REPORTING",
            $"Report Generated : {report.ReportNumber}");

        return report;
    }
}