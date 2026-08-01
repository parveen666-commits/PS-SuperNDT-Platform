using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportValidationService
{
    public bool Validate(
        ReportDataModel report)
    {
        if (report == null)
            return false;


        if (string.IsNullOrWhiteSpace(
                report.ReportNumber))
        {
            return false;
        }


        if (string.IsNullOrWhiteSpace(
                report.Customer))
        {
            return false;
        }


        if (string.IsNullOrWhiteSpace(
                report.Component))
        {
            return false;
        }


        if (report.Findings == null)
        {
            return false;
        }


        return true;
    }
}