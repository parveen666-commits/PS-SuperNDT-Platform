using System;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportValidationService
{
    public bool Validate(ReportDataModel report)
    {
        if (report == null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(report.ReportNumber))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(report.Customer))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(report.Component))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(report.Operator))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(report.Inspector))
        {
            return false;
        }

        if (report.InspectionDate > DateTime.Now)
        {
            return false;
        }

        return true;
    }
}