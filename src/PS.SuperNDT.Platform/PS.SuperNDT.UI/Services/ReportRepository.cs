using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PS.SuperNDT.UI.Database;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportRepository
{
    public IReadOnlyList<ReportDataModel> GetAll()
    {
        using var db = new SuperNDTDbContext();

        return db.Set<ReportDataModel>()
                 .Include(x => x.Findings)
                 .Include(x => x.Images)
                 .OrderByDescending(x => x.GeneratedDate)
                 .ToList();
    }

    public ReportDataModel? Get(Guid id)
    {
        using var db = new SuperNDTDbContext();

        return db.Set<ReportDataModel>()
                 .Include(x => x.Findings)
                 .Include(x => x.Images)
                 .FirstOrDefault(x => x.Id == id);
    }

    public void Save(ReportDataModel report)
    {
        using var db = new SuperNDTDbContext();

        var existing = db.Set<ReportDataModel>()
                         .FirstOrDefault(x => x.Id == report.Id);

        if (existing == null)
        {
            db.Set<ReportDataModel>().Add(report);
        }
        else
        {
            db.Entry(existing).CurrentValues.SetValues(report);
        }

        db.SaveChanges();
    }

    public void Delete(Guid id)
    {
        using var db = new SuperNDTDbContext();

        var report = db.Set<ReportDataModel>()
                       .FirstOrDefault(x => x.Id == id);

        if (report == null)
            return;

        db.Remove(report);

        db.SaveChanges();
    }
}