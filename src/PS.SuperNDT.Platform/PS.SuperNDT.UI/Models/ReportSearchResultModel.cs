using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportSearchResultModel
{
    public Guid Id { get; set; }

    public string ReportNumber { get; set; } = string.Empty;

    public string JobNumber { get; set; } = string.Empty;

    public string Customer { get; set; } = string.Empty;

    public string Component { get; set; } = string.Empty;

    public string InspectionNumber { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Result { get; set; } = string.Empty;

    public DateTime CreatedOn { get; set; }

    public string CreatedBy { get; set; } = string.Empty;

    public bool IsApproved { get; set; }

    public bool IsArchived { get; set; }
}