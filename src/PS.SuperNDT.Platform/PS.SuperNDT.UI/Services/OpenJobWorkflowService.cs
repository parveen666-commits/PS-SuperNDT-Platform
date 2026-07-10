using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class OpenJobWorkflowService
{
    private readonly OpenJobService _openJobService;
    private readonly AuditLogService _auditLogService;

    public OpenJobWorkflowService(
        OpenJobService openJobService,
        AuditLogService auditLogService)
    {
        _openJobService = openJobService;
        _auditLogService = auditLogService;
    }

    public JobModel? OpenJob(
        string jobNumber,
        string username)
    {
        var job =
            _openJobService.GetByJobNumber(jobNumber);

        if (job == null)
        {
            return null;
        }

        CurrentJobService.Instance.SetCurrentJob(job);

        _auditLogService.Add(
            username,
            "OPEN",
            "JOB",
            $"Job Opened : {job.JobNumber}");

        return job;
    }
}