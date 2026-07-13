using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportConfigurationModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string CompanyName { get; set; } = "PS SuperNDT";

    public string ReportTitle { get; set; } = "Radiographic Testing Inspection Report";

    public string StandardReference { get; set; } = string.Empty;

    public string ApprovalAuthority { get; set; } = string.Empty;

    public bool IncludeImages { get; set; } = true;

    public bool IncludeFindings { get; set; } = true;

    public bool IncludeSignature { get; set; } = true;

    public bool IncludeAuditHistory { get; set; } = true;

    public string FooterText { get; set; } = string.Empty;

    public DateTime UpdatedOn { get; set; } = DateTime.Now;
}