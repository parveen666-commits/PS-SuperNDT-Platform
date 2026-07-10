using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportService
{
    public string GenerateJobReport(
        JobModel job,
        IEnumerable<string>? images = null)
    {
        string reportsFolder =
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Reports");

        Directory.CreateDirectory(reportsFolder);

        string reportFile =
            Path.Combine(
                reportsFolder,
                $"{job.JobNumber}_Report.txt");

        var builder = new StringBuilder();

        builder.AppendLine("PS SuperNDT Platform");
        builder.AppendLine("NDT Inspection Report");
        builder.AppendLine(new string('=', 60));

        builder.AppendLine($"Job Number : {job.JobNumber}");
        builder.AppendLine($"Customer   : {job.Customer}");
        builder.AppendLine($"Project    : {job.Project}");
        builder.AppendLine($"Component  : {job.Component}");
        builder.AppendLine($"Weld No    : {job.WeldNumber}");
        builder.AppendLine($"Operator   : {job.Operator}");
        builder.AppendLine($"Procedure  : {job.Procedure}");
        builder.AppendLine($"Material   : {job.Material}");
        builder.AppendLine($"Created On : {job.CreatedOn}");
        builder.AppendLine($"Closed     : {job.IsClosed}");

        builder.AppendLine();
        builder.AppendLine("Remarks");
        builder.AppendLine("--------------------------------------------");
        builder.AppendLine(job.Remark ?? string.Empty);

        if (images != null)
        {
            builder.AppendLine();
            builder.AppendLine("Images");
            builder.AppendLine("--------------------------------------------");

            foreach (var image in images)
            {
                builder.AppendLine(image);
            }
        }

        File.WriteAllText(
            reportFile,
            builder.ToString());

        return reportFile;
    }
}