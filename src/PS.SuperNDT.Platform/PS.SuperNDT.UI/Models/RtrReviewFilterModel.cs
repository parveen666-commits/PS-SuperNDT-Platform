using System;

namespace PS.SuperNDT.UI.Models;

public sealed class RtrReviewFilterModel
{
    public string ImageName { get; set; } = string.Empty;

    public string WeldNumber { get; set; } = string.Empty;

    public string JointNumber { get; set; } = string.Empty;

    public string WeldType { get; set; } = string.Empty;

    public string WeldingProcess { get; set; } = string.Empty;

    public string IqiType { get; set; } = string.Empty;

    public string IqiSensitivity { get; set; } = string.Empty;

    public string Filter { get; set; } = string.Empty;

    public string DefectType { get; set; } = string.Empty;

    public string AcceptanceCode { get; set; } = string.Empty;

    public string Result { get; set; } = string.Empty;

    public double? SnrMinimum { get; set; }

    public double? DensityMinimum { get; set; }

    public double? DensityMaximum { get; set; }

    public double? ContrastMinimum { get; set; }

    public double? ContrastMaximum { get; set; }

    public double? KvMinimum { get; set; }

    public double? KvMaximum { get; set; }

    public double? MaMinimum { get; set; }

    public double? MaMaximum { get; set; }

    public double? ExposureTimeMinimum { get; set; }

    public double? ExposureTimeMaximum { get; set; }

    public double? SfdMinimum { get; set; }

    public double? SfdMaximum { get; set; }

    public double? UnsharpnessMaximum { get; set; }

    public DateTime? FromDate { get; set; }

    public DateTime? ToDate { get; set; }

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
        Filter = string.Empty;
        DefectType = string.Empty;
        AcceptanceCode = string.Empty;
        Result = string.Empty;

        SnrMinimum = null;

        DensityMinimum = null;
        DensityMaximum = null;

        ContrastMinimum = null;
        ContrastMaximum = null;

        KvMinimum = null;
        KvMaximum = null;

        MaMinimum = null;
        MaMaximum = null;

        ExposureTimeMinimum = null;
        ExposureTimeMaximum = null;

        SfdMinimum = null;
        SfdMaximum = null;

        UnsharpnessMaximum = null;

        FromDate = null;
        ToDate = null;

        ReviewedOnly = false;
        AcceptedOnly = false;
        RejectedOnly = false;
    }
}