using System;
using System.Collections.Generic;
using System.Linq;
using PS.SuperNDT.UI.Database;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportAuditService
{
    public IReadOnlyList<ReportAuditModel> GetAll()
    {
        using var db =
            new SuperNDTDbContext();

        return db.Set<ReportAuditModel>()
                 .OrderByDescending(x => x.PerformedOn)
                 .ToList();
    }


    public void Record(
        Guid reportId,
        string reportNumber,
        string action,
        string description,
        string performedBy)
    {
        using var db =
            new SuperNDTDbContext();


        db.Set<ReportAuditModel>()
          .Add(
              new ReportAuditModel
              {
                  Id =
                      Guid.NewGuid(),

                  ReportId =
                      reportId,

                  ReportNumber =
                      reportNumber,

                  Action =
                      action,

                  Description =
                      description,

                  PerformedBy =
                      performedBy,

                  PerformedOn =
                      DateTime.Now
              });


        db.SaveChanges();
    }


    public void Clear()
    {
        using var db =
            new SuperNDTDbContext();


        var logs =
            db.Set<ReportAuditModel>();


        db.RemoveRange(logs);


        db.SaveChanges();
    }
}