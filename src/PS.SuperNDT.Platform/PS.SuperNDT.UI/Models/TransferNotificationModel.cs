using System;

namespace PS.SuperNDT.UI.Models;

public sealed class TransferNotificationModel
{
    public Guid Id { get; set; }

    public DateTime Timestamp { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    public string Severity { get; set; } = string.Empty;

    public bool IsAcknowledged { get; set; }

    public string JobNumber { get; set; } = string.Empty;

    public string InspectionNumber { get; set; } = string.Empty;

    public string SourceModule { get; set; } = string.Empty;
}