using System;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportExposureService
{
    public ReportExposureModel CalculateExposure(
        Guid reportId,
        double voltageKV,
        double currentMA,
        double exposureTime,
        string technique)
    {
        return new ReportExposureModel
        {
            ReportId = reportId,
            Technique = technique,
            VoltageKV = voltageKV,
            CurrentMA = currentMA,
            ExposureTime = exposureTime,
            TotalExposureMAmin =
                (currentMA * exposureTime) / 60.0
        };
    }

    public bool ValidateExposure(
        ReportExposureModel exposure)
    {
        if (exposure == null)
        {
            return false;
        }

        if (exposure.VoltageKV <= 0)
        {
            return false;
        }

        if (exposure.CurrentMA <= 0)
        {
            return false;
        }

        if (exposure.ExposureTime <= 0)
        {
            return false;
        }

        return true;
    }
}