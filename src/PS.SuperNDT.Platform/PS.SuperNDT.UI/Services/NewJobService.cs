using System;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class NewJobService
{
    public JobModel Create()
    {
        return new JobModel
        {
            Id = Guid.NewGuid(),
            JobNumber = $"JOB-{DateTime.Now:yyyyMMdd-HHmmss}",
            Customer = string.Empty,
            Project = string.Empty,
            Component = string.Empty,
            WeldNumber = string.Empty,
            Operator = string.Empty,
            Procedure = string.Empty,
            Material = string.Empty,
            Remark = string.Empty,
            CreatedOn = DateTime.Now,
            IsClosed = false
        };
    }
}