using System;
using System.Collections.Generic;
using System.Linq;
using PS.SuperNDT.UI.Database;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportSignatureService
{
    public IReadOnlyList<ReportSignatureModel> GetAll()
    {
        using var db =
            new SuperNDTDbContext();

        return db.Set<ReportSignatureModel>()
                 .OrderByDescending(x => x.SignedOn)
                 .ToList();
    }


    public IEnumerable<ReportSignatureModel> GetByReport(
        Guid reportId)
    {
        using var db =
            new SuperNDTDbContext();

        return db.Set<ReportSignatureModel>()
                 .Where(x => x.ReportId == reportId)
                 .OrderByDescending(x => x.SignedOn)
                 .ToList();
    }


    public void Add(
        ReportSignatureModel signature)
    {
        ArgumentNullException.ThrowIfNull(signature);


        using var db =
            new SuperNDTDbContext();


        if (signature.Id == Guid.Empty)
        {
            signature.Id =
                Guid.NewGuid();
        }


        signature.SignedOn =
            DateTime.Now;


        db.Set<ReportSignatureModel>()
          .Add(signature);


        db.SaveChanges();


        new ReportAuditService()
            .Record(
                signature.ReportId,
                signature.ReportNumber,
                "Digital Signature Added",
                "Report digitally signed",
                signature.SignedBy);
    }


    public void Verify(
        Guid signatureId)
    {
        using var db =
            new SuperNDTDbContext();


        var signature =
            db.Set<ReportSignatureModel>()
              .FirstOrDefault(
                  x => x.Id == signatureId);


        if (signature == null)
            return;


        signature.IsValid =
            true;


        db.SaveChanges();
    }


    public void Remove(
        Guid signatureId)
    {
        using var db =
            new SuperNDTDbContext();


        var signature =
            db.Set<ReportSignatureModel>()
              .FirstOrDefault(
                  x => x.Id == signatureId);


        if (signature == null)
            return;


        db.Remove(signature);

        db.SaveChanges();
    }


    public void Clear()
    {
        using var db =
            new SuperNDTDbContext();


        var signatures =
            db.Set<ReportSignatureModel>();


        db.RemoveRange(signatures);

        db.SaveChanges();
    }
}