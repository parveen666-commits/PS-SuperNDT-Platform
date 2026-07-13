using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PS.SuperNDT.UI.Database;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ExposureService
{
    public void Save(ExposureModel exposure)
    {
        ArgumentNullException.ThrowIfNull(exposure);

        using var db = new SuperNDTDbContext();

        var existing = db.Set<ExposureModel>()
            .FirstOrDefault(x => x.Id == exposure.Id);

        if (existing == null)
        {
            db.Set<ExposureModel>().Add(exposure);
        }
        else
        {
            db.Entry(existing)
                .CurrentValues
                .SetValues(exposure);
        }

        db.SaveChanges();
    }

    public ExposureModel? Get(Guid id)
    {
        using var db = new SuperNDTDbContext();

        return db.Set<ExposureModel>()
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);
    }

    public List<ExposureModel> GetByWeld(Guid weldId)
    {
        using var db = new SuperNDTDbContext();

        return db.Set<ExposureModel>()
            .AsNoTracking()
            .Where(x => x.WeldId == weldId)
            .OrderByDescending(x => x.ExposureDateTime)
            .ToList();
    }

    public List<ExposureModel> GetAll()
    {
        using var db = new SuperNDTDbContext();

        return db.Set<ExposureModel>()
            .AsNoTracking()
            .OrderByDescending(x => x.ExposureDateTime)
            .ToList();
    }

    public void Complete(Guid exposureId, string result)
    {
        using var db = new SuperNDTDbContext();

        var exposure = db.Set<ExposureModel>()
            .FirstOrDefault(x => x.Id == exposureId);

        if (exposure == null)
            return;

        exposure.Result = result;
        exposure.IsCompleted = true;

        db.SaveChanges();
    }

    public void Delete(Guid id)
    {
        using var db = new SuperNDTDbContext();

        var exposure = db.Set<ExposureModel>()
            .FirstOrDefault(x => x.Id == id);

        if (exposure == null)
            return;

        db.Set<ExposureModel>().Remove(exposure);

        db.SaveChanges();
    }
}