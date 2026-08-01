using System;
using System.IO;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportExportService
{
    public bool Export(ReportModel report, string destinationFile)
    {
        ArgumentNullException.ThrowIfNull(report);

        if (string.IsNullOrWhiteSpace(destinationFile))
            throw new ArgumentException("Destination file is required.", nameof(destinationFile));

        try
        {
            using StreamWriter writer = new(destinationFile, false);

            writer.WriteLine("PS SuperNDT Inspection Report");
            writer.WriteLine("----------------------------------------");
            writer.WriteLine($"Report Number : {report.ReportNumber}");
            writer.WriteLine($"Job Number    : {report.JobNumber}");
            writer.WriteLine($"Customer      : {report.Customer}");
            writer.WriteLine($"Project       : {report.Project}");
            writer.WriteLine($"Component     : {report.Component}");
            writer.WriteLine($"Weld Number   : {report.WeldNumber}");
            writer.WriteLine($"Inspector     : {report.Inspector}");
            writer.WriteLine($"Report Date   : {report.ReportDate:yyyy-MM-dd HH:mm:ss}");
            writer.WriteLine($"Result        : {report.Result}");
            writer.WriteLine();
            writer.WriteLine("Remarks");
            writer.WriteLine("----------------------------------------");
            writer.WriteLine(report.Remarks);

            return true;
        }
        catch
        {
            return false;
        }
    }
}