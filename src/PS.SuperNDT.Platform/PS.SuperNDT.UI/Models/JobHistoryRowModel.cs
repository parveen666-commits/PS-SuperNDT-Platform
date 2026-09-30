
﻿using System;

namespace PS.SuperNDT.UI.Models;

public sealed class JobHistoryRowModel
{
    public Guid JobId { get; set; }

    public string JobNumber { get; set; } = string.Empty;

    public string Customer { get; set; } = string.Empty;

    public string Project { get; set; } = string.Empty;

    public string Component { get; set; } = string.Empty;

    public string WeldNumber { get; set; } = string.Empty;

    public string Operator { get; set; } = string.Empty;

    public string Procedure { get; set; } = string.Empty;

    public string Material { get; set; } = string.Empty;

    public string Remark { get; set; } = string.Empty;

    public DateTime CreatedOn { get; set; }

    public bool IsClosed { get; set; }

    // ============================================================
    // WORK ORDER MASTER STATUS
    // ============================================================

    public string WorkOrderStatus { get; set; } = "CREATED";

    public string WorkOrderResult { get; set; } = "PENDING";

    // ============================================================
    // WORK ORDER COUNTERS
    // ============================================================

    public int TotalPipes { get; set; }

    public int CompletedShots { get; set; }

    // ============================================================
    // IMAGE / SHOT COUNTERS
    // ============================================================

    public int TotalShots { get; set; }

    public int AcceptedShots { get; set; }

    public int RejectedShots { get; set; }

    public int RepairShots { get; set; }

    public int PendingShots { get; set; }

    // ============================================================
    // DISPLAY STATUS
    // ============================================================

    public string OverallStatus
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(WorkOrderStatus))
            {
                var status =
                    WorkOrderStatus.Trim()
                                   .ToUpperInvariant();

                switch (status)
                {
                    case "CLOSED":
                        return "CLOSED";

                    case "ACCEPTED":
                        return "ACCEPTED";

                    case "REJECTED":
                        return "REJECTED";

                    case "REPAIR":
                        return "REPAIR";

                    case "REVIEW_PENDING":
                        return "PENDING";

                    case "REVIEW_IN_PROGRESS":
                        return "REVIEW";

                    case "RT_IN_PROGRESS":
                        return "RT";

                    case "IN_PROGRESS":
                        return "IN PROGRESS";

                    case "CREATED":
                        return "OPEN";
                }
            }

            // ----------------------------------------------------
            // Legacy/image fallback
            // ----------------------------------------------------

            if (TotalShots == 0)
                return IsClosed
                    ? "CLOSED"
                    : "OPEN";

            if (RepairShots > 0)
                return "REPAIR";

            if (RejectedShots > 0)
                return "REJECTED";

            if (PendingShots > 0)
                return "PENDING";

            if (AcceptedShots == TotalShots)
                return "ACCEPTED";

            return IsClosed
                ? "CLOSED"
                : "OPEN";
        }
    }
}

