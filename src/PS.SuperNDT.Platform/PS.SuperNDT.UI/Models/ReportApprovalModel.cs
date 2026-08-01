using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportApprovalModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ReportId { get; set; }

    public string ReportNumber { get; set; } = string.Empty;

    public string ApprovalLevel { get; set; } = string.Empty;

    public string Designation { get; set; } = string.Empty;

    public string SubmittedBy { get; set; } = string.Empty;

    public DateTime SubmittedOn { get; set; } = DateTime.Now;

    public bool IsApproved { get; set; }

    public string ApprovedBy { get; set; } = string.Empty;

    public DateTime? ApprovedOn { get; set; }

    public string Remarks { get; set; } = string.Empty;
}