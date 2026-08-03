using System;
using System.Globalization;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportNumberService
{
    private const string Prefix = "PSNDT";

    public string Generate()
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            "{0}-{1:yyyyMMdd}-{2}",
            Prefix,
            DateTime.Now,
            DateTime.Now.ToString("HHmmss", CultureInfo.InvariantCulture));
    }

    public bool IsValid(
        string reportNumber)
    {
        if (string.IsNullOrWhiteSpace(reportNumber))
        {
            return false;
        }

        return reportNumber.StartsWith(
            $"{Prefix}-",
            StringComparison.OrdinalIgnoreCase);
    }

    public string GenerateRevision(
        string reportNumber,
        int revision)
    {
        if (!IsValid(reportNumber))
        {
            reportNumber = Generate();
        }

        return $"{reportNumber}-R{revision:D2}";
    }

    public string GenerateDuplicate(
        string reportNumber)
    {
        if (!IsValid(reportNumber))
        {
            reportNumber = Generate();
        }

        return $"{reportNumber}-COPY";
    }
}