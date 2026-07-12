using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PS.SuperNDT.UI.Database;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class JobService
{
    public void Save(JobModel job)
    {
        ArgumentNullException.ThrowIfNull(job);

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

    public JobModel? GetByJobNumber(string jobNumber)
    {
        using var db = new SuperNDTDbContext();

        return db.Jobs
                 .AsNoTracking()
                 .FirstOrDefault(x => x.JobNumber == jobNumber);
    }

    public List<JobModel> GetAll()
    {
        using var db = new SuperNDTDbContext();

        return db.Jobs
                 .AsNoTracking()
                 .OrderByDescending(x => x.CreatedOn)
                 .ToList();
    }

    public List<JobModel> GetOpenJobs()
    {
        using var db = new SuperNDTDbContext();

        return db.Jobs
                 .AsNoTracking()
                 .Where(x => !x.IsClosed)
                 .OrderByDescending(x => x.CreatedOn)
                 .ToList();
    }

    public List<JobModel> Search(string text)
    {
        using var db = new SuperNDTDbContext();

        text ??= string.Empty;

        return db.Jobs
                 .AsNoTracking()
                 .Where(x =>
                     x.JobNumber.Contains(text) ||
                     x.Customer.Contains(text) ||
                     x.Project.Contains(text) ||
                     x.Component.Contains(text) ||
                     x.WeldNumber.Contains(text))
                 .OrderByDescending(x => x.CreatedOn)
                 .ToList();
    }

    public void CloseJob(Guid id)
    {
        using var db = new SuperNDTDbContext();

        var job = db.Jobs.FirstOrDefault(x => x.Id == id);

        if (job == null)
            return;

        job.IsClosed = true;

        db.SaveChanges();
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