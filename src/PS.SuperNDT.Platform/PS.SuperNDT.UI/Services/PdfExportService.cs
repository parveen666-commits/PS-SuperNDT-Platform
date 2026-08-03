using System;
using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PS.SuperNDT.UI.Services;

public sealed class PdfExportService
{
    public string Export(
        string reportContent,
        string reportNumber)
    {
        if (string.IsNullOrWhiteSpace(reportContent))
        {
            throw new ArgumentException(
                "Report content cannot be empty.",
                nameof(reportContent));
        }


        QuestPDF.Settings.License =
            LicenseType.Community;


        string reportsFolder =
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.MyDocuments),
                "PS SuperNDT Reports");


        Directory.CreateDirectory(
            reportsFolder);


        string fileName =
            $"Report_{reportNumber}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";


        string filePath =
            Path.Combine(
                reportsFolder,
                fileName);



        Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(
                    PageSizes.A4);


                page.Margin(
                    40);


                page.Header()
                    .Text(
                        "PS SuperNDT Platform")
                    .FontSize(20)
                    .Bold();


                page.Content()
                    .Column(column =>
                    {
                        column.Spacing(10);


                        column.Item()
                            .Text(
                                "AERB Style Inspection Report")
                            .FontSize(16)
                            .Bold();


                        column.Item()
                            .Text(
                                $"Report Number : {reportNumber}");


                        column.Item()
                            .Text(
                                $"Generated On : {DateTime.Now:dd-MMM-yyyy HH:mm:ss}");


                        column.Item()
                            .LineHorizontal(1);


                        column.Item()
                            .Text(
                                reportContent)
                            .FontSize(11);

                    });


                page.Footer()
                    .AlignCenter()
                    .Text(
                        "PS SuperNDT Platform - Confidential Inspection Report");

            });

        })
        .GeneratePdf(
            filePath);


        return filePath;
    }
}