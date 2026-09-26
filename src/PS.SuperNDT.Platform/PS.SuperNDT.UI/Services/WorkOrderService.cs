using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PS.SuperNDT.UI.Database;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class WorkOrderService
{
    private readonly PipeService _pipeService = new();
    private readonly WeldService _weldService = new();

    // ============================================================
    // WORK ORDER - SAVE
    // ============================================================

    public void Save(WorkOrderModel workOrder)
    {
        ArgumentNullException.ThrowIfNull(workOrder);

        using var db = new SuperNDTDbContext();

        var existing = db.WorkOrders
            .FirstOrDefault(x => x.Id == workOrder.Id);

        if (existing == null)
        {
            db.WorkOrders.Add(workOrder);
        }
        else
        {
            db.Entry(existing)
                .CurrentValues
                .SetValues(workOrder);
        }

        db.SaveChanges();
    }

    // ============================================================
    // WORK ORDER - UPDATE
    // ============================================================

    public void Update(WorkOrderModel workOrder)
    {
        ArgumentNullException.ThrowIfNull(workOrder);

        using var db = new SuperNDTDbContext();

        var existing = db.WorkOrders
            .FirstOrDefault(x => x.Id == workOrder.Id);

        if (existing == null)
            return;

        db.Entry(existing)
            .CurrentValues
            .SetValues(workOrder);

        db.SaveChanges();
    }

    // ============================================================
    // GET WORK ORDER
    // ============================================================

    public WorkOrderModel? Get(Guid id)
    {
        using var db = new SuperNDTDbContext();

        return db.WorkOrders
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);
    }

    // ============================================================
    // GET BY WORK ORDER NUMBER
    // ============================================================

    public WorkOrderModel? GetByWorkOrderNumber(
        string workOrderNumber)
    {
        if (string.IsNullOrWhiteSpace(workOrderNumber))
            return null;

        workOrderNumber =
            workOrderNumber.Trim();

        using var db = new SuperNDTDbContext();

        return db.WorkOrders
            .AsNoTracking()
            .FirstOrDefault(x =>
                x.WorkOrderNumber == workOrderNumber);
    }

    // ============================================================
    // GET ALL
    // ============================================================

    public List<WorkOrderModel> GetAll()
    {
        using var db = new SuperNDTDbContext();

        return db.WorkOrders
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedOn)
            .ToList();
    }

    // ============================================================
    // GET OPEN WORK ORDERS
    // ============================================================

    public List<WorkOrderModel> GetOpenWorkOrders()
    {
        using var db = new SuperNDTDbContext();

        return db.WorkOrders
            .AsNoTracking()
            .Where(x =>
                !string.Equals(
                    x.Status,
                    "CLOSED",
                    StringComparison.OrdinalIgnoreCase)
                &&
                !x.ClosedOn.HasValue)
            .OrderByDescending(x => x.CreatedOn)
            .ToList();
    }

    // ============================================================
    // SEARCH
    // ============================================================

    public List<WorkOrderModel> Search(
        string searchText)
    {
        searchText ??= string.Empty;

        searchText =
            searchText.Trim();

        using var db = new SuperNDTDbContext();

        if (string.IsNullOrWhiteSpace(searchText))
        {
            return db.WorkOrders
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedOn)
                .ToList();
        }

        return db.WorkOrders
            .AsNoTracking()
            .Where(x =>
                x.WorkOrderNumber.Contains(searchText) ||
                x.Customer.Contains(searchText) ||
                x.Project.Contains(searchText) ||
                x.Component.Contains(searchText) ||
                x.DrawingNumber.Contains(searchText) ||
                x.PurchaseOrder.Contains(searchText) ||
                x.Material.Contains(searchText))
            .OrderByDescending(x => x.CreatedOn)
            .ToList();
    }

    // ============================================================
    // DUPLICATE WORK ORDER NUMBER
    // ============================================================

    public bool ExistsWorkOrderNumber(
        string workOrderNumber,
        Guid? ignoreId = null)
    {
        if (string.IsNullOrWhiteSpace(workOrderNumber))
            return false;

        workOrderNumber =
            workOrderNumber.Trim();

        using var db = new SuperNDTDbContext();

        return db.WorkOrders.Any(x =>
            x.WorkOrderNumber == workOrderNumber &&
            (!ignoreId.HasValue ||
             x.Id != ignoreId.Value));
    }

    // ============================================================
    // START
    // ============================================================

    public void Start(
        Guid workOrderId,
        string operatorName = "")
    {
        using var db = new SuperNDTDbContext();

        var workOrder =
            db.WorkOrders
                .FirstOrDefault(x =>
                    x.Id == workOrderId);

        if (workOrder == null)
            return;

        workOrder.Status =
            "IN_PROGRESS";

        if (!workOrder.StartedOn.HasValue)
        {
            workOrder.StartedOn =
                DateTime.Now;
        }

        if (!string.IsNullOrWhiteSpace(operatorName))
        {
            workOrder.AssignedOperator =
                operatorName.Trim();
        }

        workOrder.UpdatedOn =
            DateTime.Now;

        db.SaveChanges();
    }

    // ============================================================
    // START RT
    // ============================================================

    public void StartRT(
        Guid workOrderId,
        string operatorName = "")
    {
        using var db = new SuperNDTDbContext();

        var workOrder =
            db.WorkOrders
                .FirstOrDefault(x =>
                    x.Id == workOrderId);

        if (workOrder == null)
            return;

        workOrder.Status =
            "RT_IN_PROGRESS";

        workOrder.Result =
            "PENDING";

        if (!workOrder.StartedOn.HasValue)
        {
            workOrder.StartedOn =
                DateTime.Now;
        }

        if (!string.IsNullOrWhiteSpace(operatorName))
        {
            workOrder.AssignedOperator =
                operatorName.Trim();
        }

        workOrder.UpdatedOn =
            DateTime.Now;

        db.SaveChanges();
    }

    // ============================================================
    // REVIEW START
    // ============================================================

    public void StartReview(
        Guid workOrderId,
        string inspector = "")
    {
        using var db = new SuperNDTDbContext();

        var workOrder =
            db.WorkOrders
                .FirstOrDefault(x =>
                    x.Id == workOrderId);

        if (workOrder == null)
            return;

        workOrder.Status =
            "REVIEW_IN_PROGRESS";

        if (!string.IsNullOrWhiteSpace(inspector))
        {
            workOrder.AssignedInspector =
                inspector.Trim();
        }

        workOrder.UpdatedOn =
            DateTime.Now;

        db.SaveChanges();
    }

    // ============================================================
    // UPDATE STATUS
    // ============================================================

    public void UpdateStatus(
        Guid workOrderId,
        string status)
    {
        using var db = new SuperNDTDbContext();

        var workOrder =
            db.WorkOrders
                .FirstOrDefault(x =>
                    x.Id == workOrderId);

        if (workOrder == null)
            return;

        workOrder.Status =
            string.IsNullOrWhiteSpace(status)
                ? "CREATED"
                : status.Trim().ToUpperInvariant();

        workOrder.UpdatedOn =
            DateTime.Now;

        db.SaveChanges();
    }

    // ============================================================
    // UPDATE RESULT
    // ============================================================

    public void UpdateResult(
        Guid workOrderId,
        string result,
        string inspector = "")
    {
        using var db = new SuperNDTDbContext();

        var workOrder =
            db.WorkOrders
                .FirstOrDefault(x =>
                    x.Id == workOrderId);

        if (workOrder == null)
            return;

        var finalResult =
            string.IsNullOrWhiteSpace(result)
                ? "PENDING"
                : result.Trim().ToUpperInvariant();

        switch (finalResult)
        {
            case "PASS":
            case "ACCEPT":
            case "ACCEPTED":

                workOrder.Result =
                    "PASS";

                workOrder.Status =
                    "ACCEPTED";

                workOrder.CompletedOn =
                    DateTime.Now;

                break;

            case "REPAIR":

                workOrder.Result =
                    "REPAIR";

                workOrder.Status =
                    "REPAIR";

                break;

            case "REJECT":
            case "REJECTED":

                workOrder.Result =
                    "REJECT";

                workOrder.Status =
                    "REJECTED";

                break;

            default:

                workOrder.Result =
                    "PENDING";

                workOrder.Status =
                    "REVIEW_PENDING";

                break;
        }

        if (!string.IsNullOrWhiteSpace(inspector))
        {
            workOrder.AssignedInspector =
                inspector.Trim();
        }

        workOrder.UpdatedOn =
            DateTime.Now;

        db.SaveChanges();
    }

    // ============================================================
    // ACCEPT
    // ============================================================

    public void MarkAccepted(
        Guid workOrderId,
        string inspector = "")
    {
        UpdateResult(
            workOrderId,
            "PASS",
            inspector);
    }

    // ============================================================
    // REJECT
    // ============================================================

    public void MarkRejected(
        Guid workOrderId,
        string inspector = "")
    {
        UpdateResult(
            workOrderId,
            "REJECT",
            inspector);
    }

    // ============================================================
    // PENDING
    // ============================================================

    public void MarkPending(
        Guid workOrderId)
    {
        using var db = new SuperNDTDbContext();

        var workOrder =
            db.WorkOrders
                .FirstOrDefault(x =>
                    x.Id == workOrderId);

        if (workOrder == null)
            return;

        workOrder.Status =
            "REVIEW_PENDING";

        workOrder.Result =
            "PENDING";

        workOrder.CompletedOn =
            null;

        workOrder.UpdatedOn =
            DateTime.Now;

        db.SaveChanges();
    }

    // ============================================================
    // CLOSE
    // ============================================================

    public void Close(
        Guid workOrderId)
    {
        using var db = new SuperNDTDbContext();

        var workOrder =
            db.WorkOrders
                .FirstOrDefault(x =>
                    x.Id == workOrderId);

        if (workOrder == null)
            return;

        workOrder.Status =
            "CLOSED";

        workOrder.ClosedOn =
            DateTime.Now;

        workOrder.UpdatedOn =
            DateTime.Now;

        db.SaveChanges();
    }

    // ============================================================
    // REOPEN
    // ============================================================

    public void Reopen(
        Guid workOrderId)
    {
        using var db = new SuperNDTDbContext();

        var workOrder =
            db.WorkOrders
                .FirstOrDefault(x =>
                    x.Id == workOrderId);

        if (workOrder == null)
            return;

        workOrder.Status =
            "IN_PROGRESS";

        workOrder.ClosedOn =
            null;

        workOrder.UpdatedOn =
            DateTime.Now;

        db.SaveChanges();
    }

    // ============================================================
    // PIPES
    // ============================================================

    public List<PipeModel> GetPipes(
        Guid workOrderId)
    {
        return _pipeService
            .GetByWorkOrder(workOrderId);
    }

    public PipeModel? GetPipe(
        Guid pipeId)
    {
        return _pipeService
            .Get(pipeId);
    }

    public void SavePipe(
        PipeModel pipe)
    {
        ArgumentNullException.ThrowIfNull(pipe);

        _pipeService.Save(pipe);
    }

    public void UpdatePipe(
        PipeModel pipe)
    {
        ArgumentNullException.ThrowIfNull(pipe);

        _pipeService.Update(pipe);
    }

    public void DeletePipe(
        Guid pipeId)
    {
        _pipeService.Delete(pipeId);
    }

    // ============================================================
    // PIPE NUMBER CHECK
    // ============================================================

    public bool PipeNumberExists(
        Guid workOrderId,
        string pipeNumber,
        Guid? excludeId = null)
    {
        return _pipeService.ExistsPipeNumber(
            workOrderId,
            pipeNumber,
            excludeId);
    }

    // ============================================================
    // WELDS
    // ============================================================

    public List<WeldModel> GetWelds(
        Guid workOrderId)
    {
        return _weldService
            .GetByJob(workOrderId);
    }

    public WeldModel? GetWeld(
        Guid weldId)
    {
        return _weldService
            .Get(weldId);
    }

    public void SaveWeld(
        WeldModel weld)
    {
        ArgumentNullException.ThrowIfNull(weld);

        _weldService.Save(weld);
    }

    public void UpdateWeld(
        WeldModel weld)
    {
        ArgumentNullException.ThrowIfNull(weld);

        _weldService.Update(weld);
    }

    public void DeleteWeld(
        Guid weldId)
    {
        _weldService.Delete(weldId);
    }

    // ============================================================
    // WORK ORDER SUMMARY
    // ============================================================

    public WorkOrderSummary GetSummary(
        Guid workOrderId)
    {
        var pipes =
            _pipeService
                .GetByWorkOrder(workOrderId);

        var welds =
            _weldService
                .GetByJob(workOrderId);

        return new WorkOrderSummary
        {
            WorkOrderId =
                workOrderId,

            TotalPipes =
                pipes.Count,

            PendingPipes =
                pipes.Count(x =>
                    IsStatus(
                        x.Status,
                        "PENDING")),

            AcceptedPipes =
                pipes.Count(x =>
                    x.IsAccepted),

            RepairPipes =
                pipes.Count(x =>
                    x.IsRepair),

            RejectedPipes =
                pipes.Count(x =>
                    x.IsRejected),

            TotalShots =
                pipes.Sum(x =>
                    x.TotalShots),

            CompletedShots =
                pipes.Sum(x =>
                    x.CompletedShots),

            PendingShots =
                pipes.Sum(x =>
                    x.PendingShots),

            TotalWelds =
                welds.Count,

            CompletedWelds =
                welds.Count(x =>
                    IsStatus(
                        x.InspectionStatus,
                        "ACCEPTED"))
        };
    }

    // ============================================================
    // REFRESH WORK ORDER COUNTERS
    // ============================================================

    public void RefreshSummary(
        Guid workOrderId)
    {
        var pipes =
            _pipeService
                .GetByWorkOrder(workOrderId);

        var totalPipes =
            pipes.Count;

        var totalShots =
            pipes.Sum(x => x.TotalShots);

        var completedShots =
            pipes.Sum(x => x.CompletedShots);

        var pendingShots =
            pipes.Sum(x => x.PendingShots);

        var repairPipes =
            pipes.Count(x => x.IsRepair);

        var acceptedPipes =
            pipes.Count(x => x.IsAccepted);

        var rejectedPipes =
            pipes.Count(x => x.IsRejected);

        using var db =
            new SuperNDTDbContext();

        var workOrder =
            db.WorkOrders
                .FirstOrDefault(x =>
                    x.Id == workOrderId);

        if (workOrder == null)
            return;

        workOrder.TotalPipes =
            totalPipes;

        workOrder.TotalShots =
            totalShots;

        workOrder.CompletedShots =
            completedShots;

        workOrder.PendingShots =
            pendingShots;

        workOrder.RepairPipes =
            repairPipes;

        workOrder.AcceptedPipes =
            acceptedPipes;

        workOrder.RejectedPipes =
            rejectedPipes;

        workOrder.UpdatedOn =
            DateTime.Now;

        db.SaveChanges();
    }

    // ============================================================
    // STORAGE PATHS
    // ============================================================

    public void UpdateStoragePaths(
        Guid workOrderId,
        string storagePath,
        string reviewPath,
        string diconPath)
    {
        using var db =
            new SuperNDTDbContext();

        var workOrder =
            db.WorkOrders
                .FirstOrDefault(x =>
                    x.Id == workOrderId);

        if (workOrder == null)
            return;

        workOrder.StoragePath =
            storagePath?.Trim()
            ?? string.Empty;

        workOrder.ReviewPath =
            reviewPath?.Trim()
            ?? string.Empty;

        workOrder.DiconPath =
            diconPath?.Trim()
            ?? string.Empty;

        workOrder.UpdatedOn =
            DateTime.Now;

        db.SaveChanges();
    }

    // ============================================================
    // DELETE
    // ============================================================

    public void Delete(
        Guid workOrderId)
    {
        using var db =
            new SuperNDTDbContext();

        var workOrder =
            db.WorkOrders
                .FirstOrDefault(x =>
                    x.Id == workOrderId);

        if (workOrder == null)
            return;

        db.WorkOrders.Remove(workOrder);

        db.SaveChanges();
    }

    // ============================================================
    // STATUS HELPER
    // ============================================================

    private static bool IsStatus(
        string? value,
        string expected)
    {
        return string.Equals(
            value?.Trim(),
            expected,
            StringComparison.OrdinalIgnoreCase);
    }
}

// ================================================================
// WORK ORDER SUMMARY
// ================================================================

public sealed class WorkOrderSummary
{
    public Guid WorkOrderId { get; set; }

    public int TotalPipes { get; set; }

    public int PendingPipes { get; set; }

    public int AcceptedPipes { get; set; }

    public int RepairPipes { get; set; }

    public int RejectedPipes { get; set; }

    public int TotalShots { get; set; }

    public int CompletedShots { get; set; }

    public int PendingShots { get; set; }

    public int TotalWelds { get; set; }

    public int CompletedWelds { get; set; }
}