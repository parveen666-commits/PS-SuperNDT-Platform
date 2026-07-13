using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PS.SuperNDT.UI.Database;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class PLCConnectionService
{
    public void Save(PLCConnectionModel plc)
    {
        ArgumentNullException.ThrowIfNull(plc);

        using var db = new SuperNDTDbContext();

        var existing = db.Set<PLCConnectionModel>()
            .FirstOrDefault(x => x.Id == plc.Id);

        if (existing == null)
        {
            db.Set<PLCConnectionModel>().Add(plc);
        }
        else
        {
            db.Entry(existing)
                .CurrentValues
                .SetValues(plc);
        }

        db.SaveChanges();
    }

    public PLCConnectionModel? Get(Guid id)
    {
        using var db = new SuperNDTDbContext();

        return db.Set<PLCConnectionModel>()
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);
    }

    public List<PLCConnectionModel> GetAll()
    {
        using var db = new SuperNDTDbContext();

        return db.Set<PLCConnectionModel>()
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .ToList();
    }

    public List<PLCConnectionModel> GetConnected()
    {
        using var db = new SuperNDTDbContext();

        return db.Set<PLCConnectionModel>()
            .AsNoTracking()
            .Where(x => x.IsConnected)
            .OrderBy(x => x.Name)
            .ToList();
    }

    public void UpdateStatus(Guid plcId, bool connected)
    {
        using var db = new SuperNDTDbContext();

        var plc = db.Set<PLCConnectionModel>()
            .FirstOrDefault(x => x.Id == plcId);

        if (plc == null)
            return;

        plc.IsConnected = connected;
        plc.Status = connected ? "Online" : "Offline";
        plc.LastHeartbeat = DateTime.Now;

        if (connected)
            plc.ReadCount++;

        db.SaveChanges();
    }

    public void IncrementWriteCount(Guid plcId)
    {
        using var db = new SuperNDTDbContext();

        var plc = db.Set<PLCConnectionModel>()
            .FirstOrDefault(x => x.Id == plcId);

        if (plc == null)
            return;

        plc.WriteCount++;

        db.SaveChanges();
    }

    public void IncrementErrorCount(Guid plcId)
    {
        using var db = new SuperNDTDbContext();

        var plc = db.Set<PLCConnectionModel>()
            .FirstOrDefault(x => x.Id == plcId);

        if (plc == null)
            return;

        plc.ErrorCount++;

        db.SaveChanges();
    }

    public void Delete(Guid id)
    {
        using var db = new SuperNDTDbContext();

        var plc = db.Set<PLCConnectionModel>()
            .FirstOrDefault(x => x.Id == id);

        if (plc == null)
            return;

        db.Set<PLCConnectionModel>().Remove(plc);

        db.SaveChanges();
    }
}