using System;

namespace PS.SuperNDT.UI.Models;

public sealed class AuditLogModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime Timestamp { get; set; } =
        DateTime.Now;

    public string Username { get; set; } =
        string.Empty;

    public string Action { get; set; } =
        string.Empty;

    public string Module { get; set; } =
        string.Empty;

    public string Description { get; set; } =
        string.Empty;

    public string MachineName { get; set; } =
        Environment.MachineName;
}