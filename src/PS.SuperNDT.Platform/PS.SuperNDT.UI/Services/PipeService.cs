using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PS.SuperNDT.UI.Database;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class PipeService
{
    // ============================================================
    // SAVE
    // ============================================================

    public void Save(PipeModel pipe)
    {
        ArgumentNullException.ThrowIfNull(pipe);

        using var db = new SuperNDTDbContext();

        var existing = db.Pipes
            .FirstOrDefault(x => x.Id == pipe.Id);

        if (existing == null)
        {
            db.Pipes.Add(pipe);
        }
        else
        {
            db.Entry(existing)
                .CurrentValues
                .SetValues(pipe);
        }

        db.SaveChanges();
    }

    // ============================================================
    // UPDATE
    // ============================================================

    public void Update(PipeModel pipe)
    {
        ArgumentNullException.ThrowIfNull(pipe);

        using var db = new SuperNDTDbContext();

        var existing = db.Pipes
            .FirstOrDefault(x => x.Id == pipe.Id);

        if (existing == null)
        {
            return;
        }

        existing.WorkOrderId = pipe.WorkOrderId;
        existing.PipeNumber = pipe.PipeNumber;
        existing.HeatNumber = pipe.HeatNumber;
        existing.Material = pipe.Material;
        existing.MaterialSpecification = pipe.MaterialSpecification;
        existing.DiameterMm = pipe.DiameterMm;
        existing.LengthMm = pipe.LengthMm;
        existing.ThicknessMm = pipe.ThicknessMm;
        existing.DrawingNumber = pipe.DrawingNumber;
        existing.BatchNumber = pipe.BatchNumber;

        existing.TotalWelds = pipe.TotalWelds;
        existing.CompletedWelds = pipe.CompletedWelds;

        existing.TotalShots = pipe.TotalShots;
        existing.CompletedShots = pipe.CompletedShots;
        existing.PendingShots = pipe.PendingShots;
        existing.DefectShots = pipe.DefectShots;

        existing.Status = pipe.Status;
        existing.Result = pipe.Result;

        existing.RequiresRepair = pipe.RequiresRepair;
        existing.RepairLocation = pipe.RepairLocation;
        existing.RepairRemark = pipe.RepairRemark;
        existing.RepairCycle = pipe.RepairCycle;

        existing.Operator = pipe.Operator;
        existing.Inspector = pipe.Inspector;

        existing.StoragePath = pipe.StoragePath;
        existing.ReviewPath = pipe.ReviewPath;
        existing.DiconPath = pipe.DiconPath;

        existing.RTStartedOn = pipe.RTStartedOn;
        existing.RTCompletedOn = pipe.RTCompletedOn;
        existing.ReviewStartedOn = pipe.ReviewStartedOn;
        existing.ReviewedOn = pipe.ReviewedOn;
        existing.AcceptedOn = pipe.AcceptedOn;
        existing.ClosedOn = pipe.ClosedOn;

        existing.CreatedBy = pipe.CreatedBy;
        existing.UpdatedBy = pipe.UpdatedBy;
        existing.UpdatedOn = pipe.UpdatedOn;

        existing.Remark = pipe.Remark;

        db.SaveChanges();
    }

    // ============================================================
    // GET
    // ============================================================

    public PipeModel? Get(Guid id)
    {
        using var db = new SuperNDTDbContext();

        return db.Pipes
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);
    }

    // ============================================================
    // GET BY WORK ORDER
    // ============================================================

    public List<PipeModel> GetByWorkOrder(Guid workOrderId)
    {
        using var db = new SuperNDTDbContext();

        return db.Pipes
            .AsNoTracking()
            .Where(x => x.WorkOrderId == workOrderId)
            .OrderBy(x => x.PipeNumber)
            .ToList();
    }

    // ============================================================
    // GET ALL
    // ============================================================

    public List<PipeModel> GetAll()
    {
        using var db = new SuperNDTDbContext();

        return db.Pipes
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedOn)
            .ToList();
    }

    // ============================================================
    // SEARCH
    // ============================================================

    public List<PipeModel> Search(
        Guid workOrderId,
        string text)
    {
        using var db = new SuperNDTDbContext();

        text ??= string.Empty;
        text = text.Trim();

        return db.Pipes
            .AsNoTracking()
            .Where(x => x.WorkOrderId == workOrderId)
            .Where(x =>
                string.IsNullOrEmpty(text) ||
                x.PipeNumber.Contains(text) ||
                x.HeatNumber.Contains(text) ||
                x.Material.Contains(text) ||
                x.MaterialSpecification.Contains(text) ||
                x.DrawingNumber.Contains(text) ||
                x.BatchNumber.Contains(text))
            .OrderBy(x => x.PipeNumber)
            .ToList();
    }

    // ============================================================
    // GET BY PIPE NUMBER
    // ============================================================

    public PipeModel? GetByPipeNumber(
        Guid workOrderId,
        string pipeNumber)
    {
        using var db = new SuperNDTDbContext();

        pipeNumber ??= string.Empty;
        pipeNumber = pipeNumber.Trim();

        return db.Pipes
            .AsNoTracking()
            .FirstOrDefault(x =>
                x.WorkOrderId == workOrderId &&
                x.PipeNumber == pipeNumber);
    }

    // ============================================================
    // CHECK DUPLICATE PIPE NUMBER
    // ============================================================

    public bool ExistsPipeNumber(
        Guid workOrderId,
        string pipeNumber,
        Guid? ignorePipeId = null)
    {
        using var db = new SuperNDTDbContext();

        pipeNumber ??= string.Empty;
        pipeNumber = pipeNumber.Trim();

        return db.Pipes.Any(x =>
            x.WorkOrderId == workOrderId &&
            x.PipeNumber == pipeNumber &&
            (!ignorePipeId.HasValue ||
             x.Id != ignorePipeId.Value));
    }

    // ============================================================
    // STATUS
    // ============================================================

    public void UpdateStatus(
        Guid pipeId,
        string status)
    {
        using var db = new SuperNDTDbContext();

        var pipe = db.Pipes
            .FirstOrDefault(x => x.Id == pipeId);

        if (pipe == null)
        {
            return;
        }

        pipe.Status =
            string.IsNullOrWhiteSpace(status)
                ? "PENDING"
                : status.Trim().ToUpperInvariant();

        pipe.UpdatedOn = DateTime.Now;

        db.SaveChanges();
    }

    // ============================================================
    // RESULT
    // ============================================================

    public void UpdateResult(
        Guid pipeId,
        string result,
        string inspector = "")
    {
        using var db = new SuperNDTDbContext();

        var pipe = db.Pipes
            .FirstOrDefault(x => x.Id == pipeId);

        if (pipe == null)
        {
            return;
        }

        string finalResult =
            string.IsNullOrWhiteSpace(result)
                ? "PENDING"
                : result.Trim().ToUpperInvariant();

        switch (finalResult)
        {
            case "PASS":
            case "ACCEPT":
            case "ACCEPTED":

                pipe.Result = "PASS";
                pipe.Status = "ACCEPTED";
                pipe.RequiresRepair = false;
                pipe.AcceptedOn = DateTime.Now;

                break;

            case "REPAIR":

                pipe.Result = "REPAIR";
                pipe.Status = "REPAIR";
                pipe.RequiresRepair = true;

                break;

            case "REJECT":
            case "REJECTED":

                pipe.Result = "REJECT";
                pipe.Status = "REJECTED";
                pipe.RequiresRepair = false;

                break;

            default:

                pipe.Result = "PENDING";
                pipe.Status = "REVIEW_PENDING";

                break;
        }

        if (!string.IsNullOrWhiteSpace(inspector))
        {
            pipe.Inspector = inspector.Trim();
        }

        pipe.ReviewedOn = DateTime.Now;
        pipe.UpdatedOn = DateTime.Now;

        db.SaveChanges();
    }

    // ============================================================
    // START RT
    // ============================================================

    public void StartRT(
        Guid pipeId,
        string operatorName = "")
    {
        using var db = new SuperNDTDbContext();

        var pipe = db.Pipes
            .FirstOrDefault(x => x.Id == pipeId);

        if (pipe == null)
        {
            return;
        }

        pipe.Status = "RT_IN_PROGRESS";
        pipe.Result = "PENDING";
        pipe.RTStartedOn = DateTime.Now;

        if (!string.IsNullOrWhiteSpace(operatorName))
        {
            pipe.Operator = operatorName.Trim();
        }

        pipe.UpdatedOn = DateTime.Now;

        db.SaveChanges();
    }

    // ============================================================
    // COMPLETE RT
    // ============================================================

    public void CompleteRT(Guid pipeId)
    {
        using var db = new SuperNDTDbContext();

        var pipe = db.Pipes
            .FirstOrDefault(x => x.Id == pipeId);

        if (pipe == null)
        {
            return;
        }

        pipe.Status = "REVIEW_PENDING";
        pipe.Result = "PENDING";
        pipe.RTCompletedOn = DateTime.Now;
        pipe.UpdatedOn = DateTime.Now;

        db.SaveChanges();
    }

    // ============================================================
    // REVIEW START
    // ============================================================

    public void StartReview(
        Guid pipeId,
        string inspector = "")
    {
        using var db = new SuperNDTDbContext();

        var pipe = db.Pipes
            .FirstOrDefault(x => x.Id == pipeId);

        if (pipe == null)
        {
            return;
        }

        pipe.Status = "REVIEW_IN_PROGRESS";
        pipe.ReviewStartedOn = DateTime.Now;

        if (!string.IsNullOrWhiteSpace(inspector))
        {
            pipe.Inspector = inspector.Trim();
        }

        pipe.UpdatedOn = DateTime.Now;

        db.SaveChanges();
    }

    // ============================================================
    // REPAIR
    // ============================================================

    public void MarkRepair(
        Guid pipeId,
        string repairLocation,
        string repairRemark = "")
    {
        using var db = new SuperNDTDbContext();

        var pipe = db.Pipes
            .FirstOrDefault(x => x.Id == pipeId);

        if (pipe == null)
        {
            return;
        }

        pipe.Status = "REPAIR";
        pipe.Result = "REPAIR";
        pipe.RequiresRepair = true;

        pipe.RepairLocation =
            repairLocation?.Trim() ?? string.Empty;

        pipe.RepairRemark =
            repairRemark?.Trim() ?? string.Empty;

        pipe.RepairCycle++;

        pipe.ReviewedOn = DateTime.Now;
        pipe.UpdatedOn = DateTime.Now;

        db.SaveChanges();
    }

    // ============================================================
    // ACCEPT
    // ============================================================

    public void MarkAccepted(
        Guid pipeId,
        string inspector = "")
    {
        UpdateResult(
            pipeId,
            "PASS",
            inspector);
    }

    // ============================================================
    // REJECT
    // ============================================================

    public void MarkRejected(
        Guid pipeId,
        string inspector = "")
    {
        UpdateResult(
            pipeId,
            "REJECT",
            inspector);
    }

    // ============================================================
    // PENDING
    // ============================================================

    public void MarkPending(Guid pipeId)
    {
        using var db = new SuperNDTDbContext();

        var pipe = db.Pipes
            .FirstOrDefault(x => x.Id == pipeId);

        if (pipe == null)
        {
            return;
        }

        pipe.Status = "REVIEW_PENDING";
        pipe.Result = "PENDING";
        pipe.ReviewedOn = null;
        pipe.UpdatedOn = DateTime.Now;

        db.SaveChanges();
    }

    // ============================================================
    // REPAIR COMPLETED / READY FOR RE-RT
    // ============================================================

    public void CompleteRepair(Guid pipeId)
    {
        using var db = new SuperNDTDbContext();

        var pipe = db.Pipes
            .FirstOrDefault(x => x.Id == pipeId);

        if (pipe == null)
        {
            return;
        }

        pipe.Status = "RT_IN_PROGRESS";
        pipe.Result = "PENDING";
        pipe.RequiresRepair = false;
        pipe.UpdatedOn = DateTime.Now;

        db.SaveChanges();
    }

    // ============================================================
    // SHOT COUNTERS
    // ============================================================

    public void UpdateShotSummary(
        Guid pipeId,
        int totalShots,
        int completedShots,
        int pendingShots,
        int defectShots)
    {
        using var db = new SuperNDTDbContext();

        var pipe = db.Pipes
            .FirstOrDefault(x => x.Id == pipeId);

        if (pipe == null)
        {
            return;
        }

        pipe.TotalShots = Math.Max(0, totalShots);
        pipe.CompletedShots = Math.Max(0, completedShots);
        pipe.PendingShots = Math.Max(0, pendingShots);
        pipe.DefectShots = Math.Max(0, defectShots);
        pipe.UpdatedOn = DateTime.Now;

        db.SaveChanges();
    }

    // ============================================================
    // WELD COUNTERS
    // ============================================================

    public void UpdateWeldSummary(
        Guid pipeId,
        int totalWelds,
        int completedWelds)
    {
        using var db = new SuperNDTDbContext();

        var pipe = db.Pipes
            .FirstOrDefault(x => x.Id == pipeId);

        if (pipe == null)
        {
            return;
        }

        pipe.TotalWelds = Math.Max(0, totalWelds);
        pipe.CompletedWelds = Math.Max(0, completedWelds);
        pipe.UpdatedOn = DateTime.Now;

        db.SaveChanges();
    }

    // ============================================================
    // STORAGE PATHS
    // ============================================================

    public void UpdateStoragePaths(
        Guid pipeId,
        string storagePath,
        string reviewPath,
        string diconPath)
    {
        using var db = new SuperNDTDbContext();

        var pipe = db.Pipes
            .FirstOrDefault(x => x.Id == pipeId);

        if (pipe == null)
        {
            return;
        }

        pipe.StoragePath =
            storagePath?.Trim() ?? string.Empty;

        pipe.ReviewPath =
            reviewPath?.Trim() ?? string.Empty;

        pipe.DiconPath =
            diconPath?.Trim() ?? string.Empty;

        pipe.UpdatedOn = DateTime.Now;

        db.SaveChanges();
    }

    // ============================================================
    // CLOSE
    // ============================================================

    public void Close(Guid pipeId)
    {
        using var db = new SuperNDTDbContext();

        var pipe = db.Pipes
            .FirstOrDefault(x => x.Id == pipeId);

        if (pipe == null)
        {
            return;
        }

        /*
         * PipeModel does not contain IsClosed.
         *
         * Therefore CLOSED is represented by Status.
         * ClosedOn remains the audit timestamp.
         */
        pipe.Status = "CLOSED";

        pipe.ClosedOn = DateTime.Now;
        pipe.UpdatedOn = DateTime.Now;

        db.SaveChanges();
    }

    // ============================================================
    // DELETE
    // ============================================================

    public void Delete(Guid pipeId)
    {
        using var db = new SuperNDTDbContext();

        var pipe = db.Pipes
            .FirstOrDefault(x => x.Id == pipeId);

        if (pipe == null)
        {
            return;
        }

        db.Pipes.Remove(pipe);

        db.SaveChanges();
    }
}