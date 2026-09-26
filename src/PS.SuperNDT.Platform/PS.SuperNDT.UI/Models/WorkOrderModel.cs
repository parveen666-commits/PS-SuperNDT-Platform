using System;

namespace PS.SuperNDT.UI.Models;

public sealed class WorkOrderModel
{
    // ============================================================
    // IDENTITY
    // ============================================================

    public Guid Id { get; set; } = Guid.NewGuid();

    /*
     * Main business identifier.
     *
     * Example:
     * WO-2026-00125
     */
    public string WorkOrderNumber { get; set; } = "";

    // ============================================================
    // CUSTOMER / PROJECT
    // ============================================================

    public string Customer { get; set; } = "";

    public string Project { get; set; } = "";

    public string Component { get; set; } = "";

    public string DrawingNumber { get; set; } = "";

    public string PurchaseOrder { get; set; } = "";

    // ============================================================
    // INSPECTION / RT
    // ============================================================

    public string Procedure { get; set; } = "";

    public string Technique { get; set; } = "";

    public string Material { get; set; } = "";

    public double NominalThicknessMm { get; set; }

    public string MaterialSpecification { get; set; } = "";

    public string InspectionStandard { get; set; } = "";

    public string AcceptanceStandard { get; set; } = "";

    // ============================================================
    // PIPE / WORK INFORMATION
    // ============================================================

    /*
     * These are master-level defaults.
     *
     * Individual pipes can override them.
     */

    public double PipeDiameterMm { get; set; }

    public double PipeLengthMm { get; set; }

    public double DefaultShotSizeMm { get; set; }

    public double DefaultOverlapPercent { get; set; }

    // ============================================================
    // STATUS
    // ============================================================

    /*
     * CREATED
     * IN_PROGRESS
     * RT_IN_PROGRESS
     * REVIEW_PENDING
     * REVIEW_IN_PROGRESS
     * REPAIR
     * ACCEPTED
     * REJECTED
     * CLOSED
     */
    public string Status { get; set; } = "CREATED";

    /*
     * Overall work-order result.
     *
     * This is deliberately separate from Status.
     */
    public string Result { get; set; } = "PENDING";

    // ============================================================
    // ASSIGNMENT
    // ============================================================

    public string AssignedOperator { get; set; } = "";

    public string AssignedInspector { get; set; } = "";

    // ============================================================
    // COUNTERS
    // ============================================================

    /*
     * Cached counters are useful for a fast dashboard/list.
     *
     * We will update these from services instead of calculating
     * large database queries every time the screen opens.
     */

    public int TotalPipes { get; set; }

    public int TotalShots { get; set; }

    public int CompletedShots { get; set; }

    public int PendingShots { get; set; }

    public int RepairPipes { get; set; }

    public int AcceptedPipes { get; set; }

    public int RejectedPipes { get; set; }

    // ============================================================
    // FILE / STORAGE
    // ============================================================

    /*
     * Root folder for this Work Order.
     *
     * Example:
     *
     * D:\PS-SuperNDT\Data\WO-2026-00125
     */
    public string StoragePath { get; set; } = "";

    /*
     * Review data is intentionally kept separate from the
     * acquisition/database area.
     */
    public string ReviewPath { get; set; } = "";

    /*
     * Final DICON/export location.
     */
    public string DiconPath { get; set; } = "";

    // ============================================================
    // DATES
    // ============================================================

    public DateTime CreatedOn { get; set; } = DateTime.Now;

    public DateTime? StartedOn { get; set; }

    public DateTime? CompletedOn { get; set; }

    public DateTime? ClosedOn { get; set; }

    // ============================================================
    // AUDIT
    // ============================================================

    public string CreatedBy { get; set; } = "";

    public string UpdatedBy { get; set; } = "";

    public DateTime? UpdatedOn { get; set; }

    // ============================================================
    // REMARKS
    // ============================================================

    public string Remark { get; set; } = "";

    public string InternalRemark { get; set; } = "";

    // ============================================================
    // HELPER
    // ============================================================

    public bool IsClosed
    {
        get
        {
            return string.Equals(
                       Status,
                       "CLOSED",
                       StringComparison.OrdinalIgnoreCase)
                   ||
                   ClosedOn.HasValue;
        }
    }
}