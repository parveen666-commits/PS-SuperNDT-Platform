using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportInspectorModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ReportId { get; set; }

    public string InspectorName { get; set; } = string.Empty;

    public string InspectorLevel { get; set; } = string.Empty;

    public string CertificationNumber { get; set; } = string.Empty;

    public string Employer { get; set; } = string.Empty;

    public DateTime InspectionDate { get; set; } = DateTime.Now;

    public string Remarks { get; set; } = string.Empty;
}