using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportExposureModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ReportId { get; set; }

    public string Technique { get; set; } = string.Empty;

    public string RadiationSource { get; set; } = string.Empty;

    public string SourceType { get; set; } = string.Empty;

    public double SourceSize { get; set; }

    public double SFD { get; set; }

    public double ExposureTime { get; set; }

    public double VoltageKV { get; set; }

    public double CurrentMA { get; set; }

    public double TotalExposureMAmin { get; set; }

    public string FilmType { get; set; } = string.Empty;

    public string ScreenType { get; set; } = string.Empty;

    public string IQIType { get; set; } = string.Empty;
}