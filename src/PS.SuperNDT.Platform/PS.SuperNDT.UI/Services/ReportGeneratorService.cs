using System;
using System.Text;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportGeneratorService
{
    public string GenerateReportSummary(ReportDataModel report)
    {
        if (report == null)
        {
            throw new ArgumentNullException(nameof(report));
        }

        var builder = new StringBuilder();

        builder.AppendLine("PS SuperNDT Platform");
        builder.AppendLine("AERB Style Inspection Report");
        builder.AppendLine("--------------------------------");

        builder.AppendLine($"Report Number : {report.ReportNumber}");
        builder.AppendLine($"Customer      : {report.Customer}");
        builder.AppendLine($"Project       : {report.Project}");
        builder.AppendLine($"Component     : {report.Component}");
        builder.AppendLine($"Weld Number   : {report.WeldNumber}");
        builder.AppendLine($"Operator      : {report.Operator}");
        builder.AppendLine($"Inspector     : {report.Inspector}");
        builder.AppendLine($"Procedure     : {report.Procedure}");
        builder.AppendLine($"Material      : {report.Material}");
        builder.AppendLine($"Technique     : {report.Technique}");

        builder.AppendLine();
        builder.AppendLine("Exposure Parameters");
        builder.AppendLine(report.ExposureParameters);

        builder.AppendLine();
        builder.AppendLine("Inspection Result");
        builder.AppendLine(report.Result);

        builder.AppendLine();
        builder.AppendLine("Findings");

        foreach (var finding in report.Findings)
        {
            builder.AppendLine(
                $"{finding.FindingNumber}. {finding.Location} - {finding.Description} - {finding.Evaluation}");
        }

        builder.AppendLine();
        builder.AppendLine($"Generated On : {report.GeneratedDate:dd-MM-yyyy HH:mm}");

        return builder.ToString();
    }
}