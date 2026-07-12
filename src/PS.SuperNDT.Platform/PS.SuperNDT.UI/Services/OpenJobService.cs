using System;
using System.Collections.Generic;
using System.Linq;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class OpenJobService
{
    private readonly JobService _jobService;

    public OpenJobService(JobService jobService)
    {
        _jobService = jobService;
    }

    public IReadOnlyList<JobModel> GetAllJobs()
    {
        return _jobService.GetAll();
    }

    public IReadOnlyList<JobModel> Search(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return _jobService.GetAll();

        return _jobService.Search(text);
    }

    public JobModel? GetByJobNumber(string jobNumber)
    {
        return _jobService.GetByJobNumber(jobNumber);
    }

    public JobModel? GetById(Guid id)
    {
        return _jobService.Get(id);
    }

    public bool Open(Guid id)
    {
        var job = _jobService.Get(id);

        if (job == null)
            return false;

        CurrentJobService.Instance.SetCurrentJob(job);

        return true;
    }

    public bool Open(string jobNumber)
    {
        var job = _jobService.GetByJobNumber(jobNumber);

        if (job == null)
            return false;

        CurrentJobService.Instance.SetCurrentJob(job);

        return true;
    }
}