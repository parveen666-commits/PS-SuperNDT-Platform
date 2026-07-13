using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportFilterModel
{
    public string ReportNumber { get; set; } = string.Empty;

    public string JobNumber { get; set; } = string.Empty;

    public string Customer { get; set; } = string.Empty;

    public string Project { get; set; } = string.Empty;

    public string InspectionNumber { get; set; } = string.Empty;

    public string Component { get; set; } = string.Empty;

    public string WeldNumber { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Operator { get; set; } = string.Empty;

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    public bool IncludeArchived { get; set; }

    public bool IncludeApproved { get; set; }
}