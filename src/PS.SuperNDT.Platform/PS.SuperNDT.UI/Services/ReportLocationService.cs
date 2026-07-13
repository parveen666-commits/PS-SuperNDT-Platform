using System;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportLocationService
{
    public ReportLocationModel Create(
        Guid reportId,
        string componentLocation,
        string weldLocation,
        string jointNumber,
        string drawingReference,
        string area)
    {
        return new ReportLocationModel
        {
            ReportId = reportId,
            ComponentLocation = componentLocation,
            WeldLocation = weldLocation,
            JointNumber = jointNumber,
            DrawingReference = drawingReference,
            Area = area
        };
    }

    public bool Validate(
        ReportLocationModel location)
    {
        if (location == null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(location.ComponentLocation))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(location.WeldLocation))
        {
            return false;
        }

        return true;
    }
}