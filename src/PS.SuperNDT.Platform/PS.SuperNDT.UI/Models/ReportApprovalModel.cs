using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportApprovalModel
{
    public Guid Id { get; set; }

    public Guid ReportId { get; set; }

    public string ApprovalLevel { get; set; } = string.Empty;

    public string ApproverName { get; set; } = string.Empty;

    public string Designation { get; set; } = string.Empty;

    public string ApproverRole { get; set; } = string.Empty;

    public string ApprovedBy { get; set; } = string.Empty;

    public bool IsApproved { get; set; }

    public DateTime? ApprovedOn { get; set; }

    public string Remarks { get; set; } = string.Empty;

    public string ApprovalRemarks { get; set; } = string.Empty;

    public string SignatureData { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
}