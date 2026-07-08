using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PS.SuperNDT.UI.Database;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class JobService
{
    public void Save(JobModel job)
    {
        using var db = new SuperNDTDbContext();

        var existing = db.Jobs.FirstOrDefault(x => x.Id == job.Id);

        if (existing == null)
        {
            db.Jobs.Add(job);
        }
        else
        {
            db.Entry(existing).CurrentValues.SetValues(job);
        }

        db.SaveChanges();
    }

    public JobModel? Get(Guid id)
    {
        using var db = new SuperNDTDbContext();

        return db.Jobs
                 .AsNoTracking()
                 .FirstOrDefault(x => x.Id == id);
    }

    public IQueryable<JobModel> GetAll()
    {
        var db = new SuperNDTDbContext();

        return db.Jobs
                 .AsNoTracking()
                 .OrderByDescending(x => x.CreatedOn);
    }

    public void Delete(Guid id)
    {
        using var db = new SuperNDTDbContext();

        var job = db.Jobs.FirstOrDefault(x => x.Id == id);

        if (job == null)
            return;

        db.Jobs.Remove(job);

        db.SaveChanges();
    }
}