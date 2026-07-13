using System;

namespace PS.SuperNDT.UI.Models;

public sealed class TransferSessionModel
{
    public Guid Id { get; set; }

    public DateTime StartedOn { get; set; }

    public DateTime? EndedOn { get; set; }

    public string SessionName { get; set; } = string.Empty;

    public string OperatorName { get; set; } = string.Empty;

    public string MachineName { get; set; } = string.Empty;

    public int TotalQueued { get; set; }

    public int TotalTransferred { get; set; }

    public int TotalFailed { get; set; }

    public long TotalBytesTransferred { get; set; }

    public bool IsActive { get; set; }

    public string Remarks { get; set; } = string.Empty;
}