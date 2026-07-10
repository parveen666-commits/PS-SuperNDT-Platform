using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class NewJobWorkflowService
{
    private readonly NewJobService _newJobService;
    private readonly AuditLogService _auditLogService;

    public NewJobWorkflowService(
        NewJobService newJobService,
        AuditLogService auditLogService)
    {
        _newJobService = newJobService;
        _auditLogService = auditLogService;
    }

    public JobModel CreateNewJob(string username)
    {
        var job = _newJobService.Create();

        CurrentJobService.Instance.SetCurrentJob(job);

        _auditLogService.Add(
            username,
            "CREATE",
            "JOB",
            $"New Job Created : {job.JobNumber}");

        return job;
    }
}