using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportApprovalHistoryModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ReportId { get; set; }

    public string ApprovalStage { get; set; } = string.Empty;

    public string ApprovedBy { get; set; } = string.Empty;

    public string Designation { get; set; } = string.Empty;

    public string Decision { get; set; } = string.Empty;

    public string Remarks { get; set; } = string.Empty;

    public DateTime ApprovedOn { get; set; } = DateTime.Now;
}