using System;
using System.Collections.Generic;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportDataModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid JobId { get; set; }

    public string ReportNumber { get; set; } = string.Empty;

    public string Customer { get; set; } = string.Empty;

    public string Project { get; set; } = string.Empty;

    public string Component { get; set; } = string.Empty;

    public string WeldNumber { get; set; } = string.Empty;

    public string Operator { get; set; } = string.Empty;

    public string Inspector { get; set; } = string.Empty;

    public string Procedure { get; set; } = string.Empty;

    public string Material { get; set; } = string.Empty;

    public string Technique { get; set; } = string.Empty;

    public string ExposureParameters { get; set; } = string.Empty;

    public string Result { get; set; } = string.Empty;

    public string Remarks { get; set; } = string.Empty;

    public DateTime InspectionDate { get; set; } = DateTime.Now;

    public DateTime GeneratedDate { get; set; } = DateTime.Now;

    public bool IsApproved { get; set; }

    public string ApprovedBy { get; set; } = string.Empty;

    public List<ReportImageModel> Images { get; set; } = new();

    public List<ReportFindingModel> Findings { get; set; } = new();
}