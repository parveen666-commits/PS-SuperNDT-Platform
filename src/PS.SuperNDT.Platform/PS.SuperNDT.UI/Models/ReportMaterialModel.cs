using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportMaterialModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ReportId { get; set; }

    public string MaterialName { get; set; } = string.Empty;

    public string MaterialGrade { get; set; } = string.Empty;

    public string MaterialSpecification { get; set; } = string.Empty;

    public double Thickness { get; set; }

    public string WeldingProcess { get; set; } = string.Empty;

    public string JointType { get; set; } = string.Empty;

    public string HeatTreatment { get; set; } = string.Empty;

    public string Remarks { get; set; } = string.Empty;
}