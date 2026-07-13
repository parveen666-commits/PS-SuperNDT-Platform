using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PS.SuperNDT.UI.Database;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class AcquisitionSessionService
{
    public void Save(AcquisitionSessionModel session)
    {
        ArgumentNullException.ThrowIfNull(session);

        using var db = new SuperNDTDbContext();

        var existing = db.Set<AcquisitionSessionModel>()
            .FirstOrDefault(x => x.Id == session.Id);

        if (existing == null)
        {
            db.Set<AcquisitionSessionModel>().Add(session);
        }
        else
        {
            db.Entry(existing)
                .CurrentValues
                .SetValues(session);
        }

        db.SaveChanges();
    }

    public AcquisitionSessionModel? Get(Guid id)
    {
        using var db = new SuperNDTDbContext();

        return db.Set<AcquisitionSessionModel>()
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);
    }

    public List<AcquisitionSessionModel> GetAll()
    {
        using var db = new SuperNDTDbContext();

        return db.Set<AcquisitionSessionModel>()
            .AsNoTracking()
            .OrderByDescending(x => x.StartTime)
            .ToList();
    }

    public List<AcquisitionSessionModel> GetActiveSessions()
    {
        using var db = new SuperNDTDbContext();

        return db.Set<AcquisitionSessionModel>()
            .AsNoTracking()
            .Where(x => !x.IsCompleted)
            .OrderByDescending(x => x.StartTime)
            .ToList();
    }

    public void StartSession(Guid sessionId)
    {
        using var db = new SuperNDTDbContext();

        var session = db.Set<AcquisitionSessionModel>()
            .FirstOrDefault(x => x.Id == sessionId);

        if (session == null)
            return;

        session.SessionStatus = "Running";
        session.StartTime = DateTime.Now;

        db.SaveChanges();
    }

    public void RegisterShot(Guid sessionId, bool accepted)
    {
        using var db = new SuperNDTDbContext();

        var session = db.Set<AcquisitionSessionModel>()
            .FirstOrDefault(x => x.Id == sessionId);

        if (session == null)
            return;

        session.CompletedShots++;

        if (accepted)
            session.AcceptedShots++;
        else
            session.RejectedShots++;

        db.SaveChanges();
    }

    public void CompleteSession(Guid sessionId)
    {
        using var db = new SuperNDTDbContext();

        var session = db.Set<AcquisitionSessionModel>()
            .FirstOrDefault(x => x.Id == sessionId);

        if (session == null)
            return;

        session.IsCompleted = true;
        session.SessionStatus = "Completed";
        session.EndTime = DateTime.Now;

        db.SaveChanges();
    }

    public void Delete(Guid id)
    {
        using var db = new SuperNDTDbContext();

        var session = db.Set<AcquisitionSessionModel>()
            .FirstOrDefault(x => x.Id == id);

        if (session == null)
            return;

        db.Set<AcquisitionSessionModel>().Remove(session);

        db.SaveChanges();
    }
}