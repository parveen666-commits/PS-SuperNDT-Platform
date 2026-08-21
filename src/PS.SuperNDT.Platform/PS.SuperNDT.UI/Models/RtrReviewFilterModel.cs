using System;

namespace PS.SuperNDT.UI.Models;

public sealed class RtrReviewFilterModel
{
    // Image / Weld

    public string ImageName { get; set; } = string.Empty;

    public string WeldNumber { get; set; } = string.Empty;

    public string JointNumber { get; set; } = string.Empty;

    public string WeldType { get; set; } = string.Empty;

    public string WeldingProcess { get; set; } = string.Empty;

    // IQI / Image Quality

    public string IqiType { get; set; } = string.Empty;

    public string IqiSensitivity { get; set; } = string.Empty;

    public double? IqiMinimum { get; set; }

    public double? IqiMaximum { get; set; }

    public string Filter { get; set; } = string.Empty;

    public string Grain { get; set; } = string.Empty;

    public double? SnrMinimum { get; set; }

    public double? SnrMaximum { get; set; }

    public double? DensityMinimum { get; set; }

    public double? DensityMaximum { get; set; }

    public double? ContrastMinimum { get; set; }

    public double? ContrastMaximum { get; set; }

    public double? BasicSpatialResolutionMaximum { get; set; }

    // Exposure

    public double? KvMinimum { get; set; }

    public double? KvMaximum { get; set; }

    public double? MaMinimum { get; set; }

    public double? MaMaximum { get; set; }

    public double? ExposureTimeMinimum { get; set; }

    public double? ExposureTimeMaximum { get; set; }

    // Geometry

    public double? SfdMinimum { get; set; }

    public double? SfdMaximum { get; set; }

    public double? OddMinimum { get; set; }

    public double? OddMaximum { get; set; }

    public double? UnsharpnessMaximum { get; set; }

    // Weld / Material

    public double? MaterialThicknessMinimum { get; set; }

    public double? MaterialThicknessMaximum { get; set; }

    // Defect

    public string DefectType { get; set; } = string.Empty;

    public string AcceptanceCode { get; set; } = string.Empty;

    public string Result { get; set; } = string.Empty;

    // Date

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

    // Review Status

    public bool ReviewedOnly { get; set; }

    public bool AcceptedOnly { get; set; }

    public bool RejectedOnly { get; set; }

    public void Clear()
    {
        ImageName = string.Empty;

        WeldNumber = string.Empty;

        JointNumber = string.Empty;

        WeldType = string.Empty;

        WeldingProcess = string.Empty;

        IqiType = string.Empty;

        IqiSensitivity = string.Empty;

        IqiMinimum = null;

        IqiMaximum = null;

        Filter = string.Empty;

        Grain = string.Empty;

        SnrMinimum = null;

        SnrMaximum = null;

        DensityMinimum = null;

        DensityMaximum = null;

        ContrastMinimum = null;

        ContrastMaximum = null;

        BasicSpatialResolutionMaximum = null;

        KvMinimum = null;

        KvMaximum = null;

        MaMinimum = null;

        MaMaximum = null;

        ExposureTimeMinimum = null;

        ExposureTimeMaximum = null;

        SfdMinimum = null;

        SfdMaximum = null;

        OddMinimum = null;

        OddMaximum = null;

        UnsharpnessMaximum = null;

        MaterialThicknessMinimum = null;

        MaterialThicknessMaximum = null;

        DefectType = string.Empty;

        AcceptanceCode = string.Empty;

        Result = string.Empty;

        FromDate = null;

        ToDate = null;

        ReviewedOnly = false;

        AcceptedOnly = false;

        RejectedOnly = false;
    }
}