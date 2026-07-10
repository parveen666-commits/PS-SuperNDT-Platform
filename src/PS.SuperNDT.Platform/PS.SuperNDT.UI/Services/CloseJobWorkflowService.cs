using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class CloseJobWorkflowService
{
    private readonly AuditLogService _auditLogService;

    public CloseJobWorkflowService(
        AuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    public bool CloseCurrentJob(
        string username)
    {
        JobModel? currentJob =
            CurrentJobService.Instance.CurrentJob;

        if (currentJob == null)
        {
            return false;
        }

        string jobNumber =
            currentJob.JobNumber;

        CurrentJobService.Instance.CloseCurrentJob();

        _auditLogService.Add(
            username,
            "CLOSE",
            "JOB",
            $"Job Closed : {jobNumber}");

        return true;
    }
}