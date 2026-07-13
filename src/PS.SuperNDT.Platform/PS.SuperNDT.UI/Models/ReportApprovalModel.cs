using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportApprovalModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ReportId { get; set; }

    public string ApprovalLevel { get; set; } = string.Empty;

    public string ApprovedBy { get; set; } = string.Empty;

    public string Designation { get; set; } = string.Empty;

    public DateTime ApprovedOn { get; set; } = DateTime.Now;

    public bool IsApproved { get; set; }

    public string Remarks { get; set; } = string.Empty;
}