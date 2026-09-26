using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PS.SuperNDT.UI.Database;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class WeldService
{
    // ============================================================
    // SAVE
    // ============================================================

    public void Save(WeldModel weld)
    {
        ArgumentNullException.ThrowIfNull(weld);

        using var db = new SuperNDTDbContext();

        var existing =
            db.Set<WeldModel>()
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

    // ============================================================
    // UPDATE
    // ============================================================

    public void Update(WeldModel weld)
    {
        ArgumentNullException.ThrowIfNull(weld);

        using var db = new SuperNDTDbContext();

        var existing =
            db.Set<WeldModel>()
              .FirstOrDefault(x => x.Id == weld.Id);

        if (existing == null)
        {
            return;
        }

        existing.WeldNumber =
            weld.WeldNumber;

        existing.SpoolNumber =
            weld.SpoolNumber;

        existing.LineNumber =
            weld.LineNumber;

        existing.JointType =
            weld.JointType;

        existing.Material =
            weld.Material;

        existing.Diameter =
            weld.Diameter;

        existing.Thickness =
            weld.Thickness;

        existing.Schedule =
            weld.Schedule;

        existing.Technique =
            weld.Technique;

        existing.InspectionStatus =
            weld.InspectionStatus;

        existing.Remarks =
            weld.Remarks;

        db.SaveChanges();
    }

    // ============================================================
    // GET
    // ============================================================

    public WeldModel? Get(Guid id)
    {
        using var db = new SuperNDTDbContext();

        return db.Set<WeldModel>()
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);
    }

    // ============================================================
    // GET BY JOB
    // ============================================================

    public List<WeldModel> GetByJob(Guid jobId)
    {
        using var db = new SuperNDTDbContext();

        return db.Set<WeldModel>()
            .AsNoTracking()
            .Where(x => x.JobId == jobId)
            .OrderBy(x => x.WeldNumber)
            .ThenBy(x => x.SpoolNumber)
            .ToList();
    }

    // ============================================================
    // GET ALL
    // ============================================================

    public List<WeldModel> GetAll()
    {
        using var db = new SuperNDTDbContext();

        return db.Set<WeldModel>()
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedOn)
            .ToList();
    }

    // ============================================================
    // SEARCH WITHIN JOB
    // ============================================================

    public List<WeldModel> Search(
        Guid jobId,
        string text)
    {
        using var db = new SuperNDTDbContext();

        text ??= string.Empty;

        return db.Set<WeldModel>()
            .AsNoTracking()
            .Where(x =>
                x.JobId == jobId &&
                (
                    x.WeldNumber.Contains(text) ||
                    x.SpoolNumber.Contains(text) ||
                    x.LineNumber.Contains(text) ||
                    x.Material.Contains(text) ||
                    x.JointType.Contains(text)
                ))
            .OrderBy(x => x.WeldNumber)
            .ToList();
    }

    // ============================================================
    // DUPLICATE CHECK
    // ============================================================

    public bool ExistsWeldNumber(
        Guid jobId,
        string weldNumber,
        Guid? excludeId = null)
    {
        using var db = new SuperNDTDbContext();

        weldNumber ??= string.Empty;

        var query =
            db.Set<WeldModel>()
              .AsNoTracking()
              .Where(x =>
                  x.JobId == jobId &&
                  x.WeldNumber == weldNumber);

        if (excludeId.HasValue)
        {
            query =
                query.Where(
                    x => x.Id != excludeId.Value);
        }

        return query.Any();
    }

    // ============================================================
    // SHOT COUNTS
    // ============================================================

    public void UpdateShotResult(
        Guid weldId,
        bool accepted)
    {
        using var db = new SuperNDTDbContext();

        var weld =
            db.Set<WeldModel>()
              .FirstOrDefault(x => x.Id == weldId);

        if (weld == null)
        {
            return;
        }

        weld.TotalShots++;

        if (accepted)
        {
            weld.AcceptedShots++;
        }
        else
        {
            weld.RejectedShots++;
        }

        db.SaveChanges();
    }

    // ============================================================
    // RECALCULATE SHOT COUNTS
    //
    // Later this will read actual ImageRecordModel shots.
    // Keeping it here gives us one central place for the logic.
    // ============================================================

    public void RecalculateShotCounts(
        Guid weldId)
    {
        using var db = new SuperNDTDbContext();

        var weld =
            db.Set<WeldModel>()
              .FirstOrDefault(x => x.Id == weldId);

        if (weld == null)
        {
            return;
        }

        var shots =
            db.Set<ImageRecordModel>()
              .AsNoTracking()
              .Where(x =>
                  x.JobId == weld.JobId &&
                  x.WeldNumber == weld.WeldNumber)
              .ToList();

        weld.TotalShots =
            shots.Count;

        weld.AcceptedShots =
            shots.Count(x =>
                string.Equals(
                    x.ReviewStatus,
                    "ACCEPTED",
                    StringComparison.OrdinalIgnoreCase));

        weld.RejectedShots =
            shots.Count(x =>
                string.Equals(
                    x.ReviewStatus,
                    "REJECTED",
                    StringComparison.OrdinalIgnoreCase));

        if (shots.Count == 0)
        {
            weld.InspectionStatus =
                "PENDING";
        }
        else if (shots.Any(x =>
            string.Equals(
                x.ReviewStatus,
                "REJECTED",
                StringComparison.OrdinalIgnoreCase)))
        {
            weld.InspectionStatus =
                "REPAIR";
        }
        else if (shots.All(x =>
            string.Equals(
                x.ReviewStatus,
                "ACCEPTED",
                StringComparison.OrdinalIgnoreCase)))
        {
            weld.InspectionStatus =
                "ACCEPTED";
        }
        else
        {
            weld.InspectionStatus =
                "PENDING";
        }

        db.SaveChanges();
    }

    // ============================================================
    // STATUS
    // ============================================================

    public void SetInspectionStatus(
        Guid weldId,
        string status)
    {
        using var db = new SuperNDTDbContext();

        var weld =
            db.Set<WeldModel>()
              .FirstOrDefault(x => x.Id == weldId);

        if (weld == null)
        {
            return;
        }

        weld.InspectionStatus =
            string.IsNullOrWhiteSpace(status)
                ? "PENDING"
                : status.Trim().ToUpperInvariant();

        db.SaveChanges();
    }

    // ============================================================
    // DELETE
    // ============================================================

    public void Delete(Guid id)
    {
        using var db = new SuperNDTDbContext();

        var weld =
            db.Set<WeldModel>()
              .FirstOrDefault(x => x.Id == id);

        if (weld == null)
        {
            return;
        }

        db.Set<WeldModel>()
          .Remove(weld);

        db.SaveChanges();
    }

    // ============================================================
    // DELETE ALL WELDS OF A JOB
    // ============================================================

    public void DeleteByJob(Guid jobId)
    {
        using var db = new SuperNDTDbContext();

        var welds =
            db.Set<WeldModel>()
              .Where(x => x.JobId == jobId)
              .ToList();

        if (welds.Count == 0)
        {
            return;
        }

        db.Set<WeldModel>()
          .RemoveRange(welds);

        db.SaveChanges();
    }
}