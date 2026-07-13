using System;

namespace PS.SuperNDT.UI.Models;

public sealed class TransferAuditLogModel
{
    public Guid Id { get; set; }

    public DateTime Timestamp { get; set; }

    public string EventType { get; set; } = string.Empty;

    public string JobNumber { get; set; } = string.Empty;

    public string InspectionNumber { get; set; } = string.Empty;

    public string PackageNumber { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string MachineName { get; set; } = string.Empty;

    public string Source { get; set; } = string.Empty;

    public string Destination { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}