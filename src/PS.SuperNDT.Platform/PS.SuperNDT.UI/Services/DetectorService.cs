using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PS.SuperNDT.UI.Database;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class DetectorService
{
    public void Save(DetectorModel detector)
    {
        ArgumentNullException.ThrowIfNull(detector);

        using var db = new SuperNDTDbContext();

        var existing = db.Set<DetectorModel>()
            .FirstOrDefault(x => x.Id == detector.Id);

        if (existing == null)
        {
            db.Set<DetectorModel>().Add(detector);
        }
        else
        {
            db.Entry(existing)
                .CurrentValues
                .SetValues(detector);
        }

        db.SaveChanges();
    }

    public DetectorModel? Get(Guid id)
    {
        using var db = new SuperNDTDbContext();

        return db.Set<DetectorModel>()
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);
    }

    public List<DetectorModel> GetAll()
    {
        using var db = new SuperNDTDbContext();

        return db.Set<DetectorModel>()
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToList();
    }

    public List<DetectorModel> GetConnectedDetectors()
    {
        using var db = new SuperNDTDbContext();

        return db.Set<DetectorModel>()
            .AsNoTracking()
            .Where(x => x.IsConnected)
            .OrderBy(x => x.Name)
            .ToList();
    }

    public void UpdateConnectionStatus(Guid detectorId, bool connected)
    {
        using var db = new SuperNDTDbContext();

        var detector = db.Set<DetectorModel>()
            .FirstOrDefault(x => x.Id == detectorId);

        if (detector == null)
            return;

        detector.IsConnected = connected;
        detector.Status = connected ? "Online" : "Offline";
        detector.LastHeartbeat = DateTime.Now;

        db.SaveChanges();
    }

    public void Delete(Guid id)
    {
        using var db = new SuperNDTDbContext();

        var detector = db.Set<DetectorModel>()
            .FirstOrDefault(x => x.Id == id);

        if (detector == null)
            return;

        db.Set<DetectorModel>().Remove(detector);

        db.SaveChanges();
    }
}