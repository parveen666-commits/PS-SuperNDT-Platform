using System;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportWorkflowService
{
    private ReportWorkflowStatusModel _workflow;

    public ReportWorkflowService()
    {
        _workflow = new ReportWorkflowStatusModel();
    }

    public ReportWorkflowStatusModel GetStatus()
    {
        return _workflow;
    }

    public void StartReport(
        Guid reportId,
        string user)
    {
        _workflow.ReportId = reportId;
        _workflow.Status = "Draft";
        _workflow.CurrentStage = "Report Creation";
        _workflow.UpdatedBy = user;
        _workflow.UpdatedOn = DateTime.Now;
        _workflow.IsCompleted = false;
    }

    public void SubmitForReview(
        string user)
    {
        _workflow.Status = "Under Review";
        _workflow.CurrentStage = "Level-II Review";
        _workflow.UpdatedBy = user;
        _workflow.UpdatedOn = DateTime.Now;
    }

    public void Approve(
        string user)
    {
        _workflow.Status = "Approved";
        _workflow.CurrentStage = "Level-III Approval";
        _workflow.UpdatedBy = user;
        _workflow.UpdatedOn = DateTime.Now;
    }

    public void Complete(
        string user)
    {
        _workflow.Status = "Completed";
        _workflow.CurrentStage = "Archived";
        _workflow.UpdatedBy = user;
        _workflow.UpdatedOn = DateTime.Now;
        _workflow.IsCompleted = true;
    }
}