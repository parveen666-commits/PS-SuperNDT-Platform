using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportSummaryModel
{
    public Guid Id { get; set; }

    public string ReportNumber { get; set; } = string.Empty;

    public string JobNumber { get; set; } = string.Empty;

    public string Customer { get; set; } = string.Empty;

    public string Component { get; set; } = string.Empty;

    public string InspectionMethod { get; set; } = string.Empty;

    public string Result { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public int ImageCount { get; set; }

    public int AnnotationCount { get; set; }

    public bool IsApproved { get; set; }

    public DateTime CreatedOn { get; set; }

    public string CreatedBy { get; set; } = string.Empty;
}