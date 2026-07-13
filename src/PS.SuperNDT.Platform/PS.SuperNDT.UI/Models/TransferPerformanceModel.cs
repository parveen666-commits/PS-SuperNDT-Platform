using System;

namespace PS.SuperNDT.UI.Models;

public sealed class TransferPerformanceModel
{
    public DateTime Timestamp { get; set; }

    public int TransfersCompleted { get; set; }

    public int TransfersFailed { get; set; }

    public long BytesTransferred { get; set; }

    public double AverageTransferTimeSeconds { get; set; }

    public double FastestTransferSeconds { get; set; }

    public double SlowestTransferSeconds { get; set; }

    public string BestPerformancePeriod { get; set; } = string.Empty;

    public string WorstPerformancePeriod { get; set; } = string.Empty;

    public double SuccessRate { get; set; }

    public double FailureRate { get; set; }
}