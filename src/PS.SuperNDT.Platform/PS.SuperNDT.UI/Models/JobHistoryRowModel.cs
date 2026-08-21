using System;

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

    public int TotalShots { get; set; }

    public int AcceptedShots { get; set; }

    public int RejectedShots { get; set; }

    public int RepairShots { get; set; }

    public int PendingShots { get; set; }

    public string OverallStatus
    {
        get
        {
            if (TotalShots == 0)
                return IsClosed ? "CLOSED" : "OPEN";

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