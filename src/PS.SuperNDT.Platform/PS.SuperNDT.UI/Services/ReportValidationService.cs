
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportValidationService
{
    public string LastValidationMessage { get; private set; } =
        string.Empty;

    public bool Validate(
        ReportDataModel report)
    {
        LastValidationMessage =
            string.Empty;

        if (report == null)
        {
            LastValidationMessage =
                "Report data is not available.";

            return false;
        }

        if (string.IsNullOrWhiteSpace(
                report.ReportNumber))
        {
            LastValidationMessage =
                "Report Number is required.";

            return false;
        }

        if (string.IsNullOrWhiteSpace(
                report.Customer))
        {
            LastValidationMessage =
                "Customer is required.";

            return false;
        }

        if (string.IsNullOrWhiteSpace(
                report.Component))
        {
            LastValidationMessage =
                "Component is required.";

            return false;
        }

        if (report.Findings == null)
        {
            LastValidationMessage =
                "Report findings are not available.";

            return false;
        }

        LastValidationMessage =
            "Report validation successful.";

        return true;
    }
}

