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

    public JobModel? GetByJobNumber(string jobNumber)
    {
        return _jobService
            .GetAll()
            .FirstOrDefault(x => x.JobNumber == jobNumber);
    }

    public JobModel? GetById(System.Guid id)
    {
        return _jobService
            .GetAll()
            .FirstOrDefault(x => x.Id == id);
    }
}