using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PS.SuperNDT.UI.Database;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class WeldService
{
    public void Save(WeldModel weld)
    {
        ArgumentNullException.ThrowIfNull(weld);

        using var db = new SuperNDTDbContext();

        var existing = db.Set<WeldModel>()
            .FirstOrDefault(x => x.Id == weld.Id);

        if (existing == null)
        {
            db.Set<WeldModel>().Add(weld);
        }
        else
        {
            db.Entry(existing)
              .CurrentValues
              .SetValues(weld);
        }

        db.SaveChanges();
    }

    public WeldModel? Get(Guid id)
    {
        using var db = new SuperNDTDbContext();

        return db.Set<WeldModel>()
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);
    }

    public List<WeldModel> GetByJob(Guid jobId)
    {
        using var db = new SuperNDTDbContext();

        return db.Set<WeldModel>()
            .AsNoTracking()
            .Where(x => x.JobId == jobId)
            .OrderBy(x => x.WeldNumber)
            .ToList();
    }

    public List<WeldModel> GetAll()
    {
        using var db = new SuperNDTDbContext();

        return db.Set<WeldModel>()
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedOn)
            .ToList();
    }

    public void UpdateShotResult(Guid weldId, bool accepted)
    {
        using var db = new SuperNDTDbContext();

        var weld = db.Set<WeldModel>()
            .FirstOrDefault(x => x.Id == weldId);

        if (weld == null)
            return;

        weld.TotalShots++;

        if (accepted)
            weld.AcceptedShots++;
        else
            weld.RejectedShots++;

        db.SaveChanges();
    }

    public void Delete(Guid id)
    {
        using var db = new SuperNDTDbContext();

        var weld = db.Set<WeldModel>()
            .FirstOrDefault(x => x.Id == id);

        if (weld == null)
            return;

        db.Set<WeldModel>().Remove(weld);

        db.SaveChanges();
    }
}