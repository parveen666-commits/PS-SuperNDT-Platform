using System;

namespace PS.SuperNDT.UI.Models;

public sealed class AcquisitionSessionModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid JobId { get; set; }

    public Guid WeldId { get; set; }

    public Guid ExposureId { get; set; }

    public string SessionNumber { get; set; } = string.Empty;

    public string OperatorName { get; set; } = string.Empty;

    public string DetectorName { get; set; } = string.Empty;

    public string PlcName { get; set; } = string.Empty;

    public int PlannedShots { get; set; }

    public int CompletedShots { get; set; }

    public int AcceptedShots { get; set; }

    public int RejectedShots { get; set; }

    public string SessionStatus { get; set; } = "Ready";

    public DateTime StartTime { get; set; } = DateTime.Now;

    public DateTime? EndTime { get; set; }

    public double AverageExposureTime { get; set; }

    public string Remarks { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    public double ProgressPercentage
    {
        get
        {
            if (PlannedShots <= 0)
                return 0;

            return (double)CompletedShots / PlannedShots * 100.0;
        }
    }
}