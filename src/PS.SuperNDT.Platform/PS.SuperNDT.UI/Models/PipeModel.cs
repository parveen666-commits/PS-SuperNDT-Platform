using System;

namespace PS.SuperNDT.UI.Models;

public sealed class PipeModel
{
    // ============================================================
    // IDENTITY
    // ============================================================

    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid WorkOrderId { get; set; }

    /*
     * Human-readable pipe number.
     *
     * Example:
     * P-001
     * P-002
     */
    public string PipeNumber { get; set; } = "";

    // ============================================================
    // PIPE INFORMATION
    // ============================================================

    public string HeatNumber { get; set; } = "";

    public string Material { get; set; } = "";

    public string MaterialSpecification { get; set; } = "";

    public double DiameterMm { get; set; }

    public double LengthMm { get; set; }

    public double ThicknessMm { get; set; }

    public string DrawingNumber { get; set; } = "";

    public string BatchNumber { get; set; } = "";

    // ============================================================
    // WELD INFORMATION
    // ============================================================

    /*
     * Total welds/joints expected on this pipe.
     *
     * Individual weld records will be added later.
     */
    public int TotalWelds { get; set; }

    public int CompletedWelds { get; set; }

    // ============================================================
    // RT / SHOT INFORMATION
    // ============================================================

    public int TotalShots { get; set; }

    public int CompletedShots { get; set; }

    public int PendingShots { get; set; }

    /*
     * Number of shots containing one or more defect marks.
     */
    public int DefectShots { get; set; }

    // ============================================================
    // PIPE STATUS
    // ============================================================

    /*
     * PIPE STATUS is the important final status.
     *
     * PENDING
     * RT_IN_PROGRESS
     * REVIEW_PENDING
     * REVIEW_IN_PROGRESS
     * REPAIR
     * ACCEPTED
     * REJECTED
     */
    public string Status { get; set; } = "PENDING";

    /*
     * Final inspection result.
     *
     * PENDING
     * PASS
     * REPAIR
     * REJECT
     */
    public string Result { get; set; } = "PENDING";

    // ============================================================
    // REPAIR INFORMATION
    // ============================================================

    /*
     * True when this pipe needs repair.
     */
    public bool RequiresRepair { get; set; }

    /*
     * Human-readable repair location.
     *
     * Example:
     * Weld W-12 / Shot 07
     */
    public string RepairLocation { get; set; } = "";

    /*
     * Inspector can add a short repair instruction.
     */
    public string RepairRemark { get; set; } = "";

    /*
     * Number of repair/re-RT cycles.
     */
    public int RepairCycle { get; set; }

    // ============================================================
    // RT ASSIGNMENT
    // ============================================================

    public string Operator { get; set; } = "";

    public string Inspector { get; set; } = "";

    // ============================================================
    // STORAGE
    // ============================================================

    /*
     * Pipe-specific storage root.
     *
     * Example:
     *
     * ...\WO-2026-00125\P-001
     */
    public string StoragePath { get; set; } = "";

    /*
     * Review folder for this pipe.
     */
    public string ReviewPath { get; set; } = "";

    /*
     * Final/export path if required.
     */
    public string DiconPath { get; set; } = "";

    // ============================================================
    // DATES
    // ============================================================

    public DateTime CreatedOn { get; set; } = DateTime.Now;

    public DateTime? RTStartedOn { get; set; }

    public DateTime? RTCompletedOn { get; set; }

    public DateTime? ReviewStartedOn { get; set; }

    public DateTime? ReviewedOn { get; set; }

    public DateTime? AcceptedOn { get; set; }

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

    // ============================================================
    // HELPERS
    // ============================================================

    public bool IsAccepted
    {
        get
        {
            return string.Equals(
                Result,
                "PASS",
                StringComparison.OrdinalIgnoreCase)
                ||
                string.Equals(
                    Status,
                    "ACCEPTED",
                    StringComparison.OrdinalIgnoreCase);
        }
    }

    public bool IsRepair
    {
        get
        {
            return RequiresRepair
                   ||
                   string.Equals(
                       Result,
                       "REPAIR",
                       StringComparison.OrdinalIgnoreCase)
                   ||
                   string.Equals(
                       Status,
                       "REPAIR",
                       StringComparison.OrdinalIgnoreCase);
        }
    }

    public bool IsRejected
    {
        get
        {
            return string.Equals(
                Result,
                "REJECT",
                StringComparison.OrdinalIgnoreCase)
                ||
                string.Equals(
                    Status,
                    "REJECTED",
                    StringComparison.OrdinalIgnoreCase);
        }
    }
}