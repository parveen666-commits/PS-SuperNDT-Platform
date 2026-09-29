using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using PS.SuperNDT.UI.Models;

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

        string filePath =
            CreateReportFilePath(reportNumber);

        Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);

                page.Header()
                    .Column(header =>
                    {
                        header.Item()
                            .Text("PS SuperNDT Platform")
                            .FontSize(20)
                            .Bold();

                        header.Item()
                            .Text("DIGITAL RADIOGRAPHY INSPECTION REPORT")
                            .FontSize(12)
                            .Bold();

                        header.Item()
                            .LineHorizontal(1);
                    });

                page.Content()
                    .Column(column =>
                    {
                        column.Spacing(10);

                        column.Item()
                            .Text("REPORT INFORMATION")
                            .FontSize(12)
                            .Bold();

                        column.Item()
                            .Text(
                                $"Report Number : {reportNumber}\n" +
                                $"Generated On  : {DateTime.Now:dd-MMM-yyyy HH:mm:ss}")
                            .FontSize(10);

                        column.Item()
                            .LineHorizontal(0.5f);

                        column.Item()
                            .Text("REPORT CONTENT")
                            .FontSize(12)
                            .Bold();

                        column.Item()
                            .Text(reportContent)
                            .FontSize(10)
                            .LineHeight(1.2f);
                    });

                AddFooter(page);
            });
        })
        .GeneratePdf(filePath);

        return filePath;
    }

    public string Export(
        ReportDataModel report)
    {
        ArgumentNullException.ThrowIfNull(report);

        QuestPDF.Settings.License =
            LicenseType.Community;

        string filePath =
            CreateReportFilePath(report.ReportNumber);

        Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(32);

                page.Header()
                    .Column(header =>
                    {
                        header.Item()
                            .Text("PS SuperNDT Platform")
                            .FontSize(17)
                            .Bold();

                        header.Item()
                            .Text("DIGITAL RADIOGRAPHY INSPECTION REPORT")
                            .FontSize(10)
                            .Bold();

                        header.Item()
                            .LineHorizontal(0.7f);
                    });

                page.Content()
                    .Column(column =>
                    {
                        column.Spacing(5);

                        AddSectionTitle(
                            column,
                            "REPORT INFORMATION");

                        AddInformationTable(
                            column,
                            report);

                        AddSectionTitle(
                            column,
                            "INSPECTION DETAILS");

                        AddInspectionDetails(
                            column,
                            report);

                        AddSectionTitle(
                            column,
                            "EXPOSURE PARAMETERS");

                        column.Item()
                            .Text(
                                GetValue(
                                    report.ExposureParameters))
                            .FontSize(8);

                        AddSectionTitle(
                            column,
                            "INSPECTION RESULT");

                        column.Item()
                            .Text(
                                GetValue(
                                    report.Result))
                            .FontSize(9);

                        if (!string.IsNullOrWhiteSpace(
                                report.Remarks))
                        {
                            AddSectionTitle(
                                column,
                                "REMARKS");

                            column.Item()
                                .Text(report.Remarks)
                                .FontSize(8);
                        }

                        AddSectionTitle(
                            column,
                            "INSPECTION FINDINGS");

                        AddFindingsTable(
                            column,
                            report);

                        if (report.Images != null &&
                            report.Images.Count > 0)
                        {
                            AddSectionTitle(
                                column,
                                "REVIEWED INSPECTION IMAGES");

                            AddImages(
                                column,
                                report);
                        }

                        AddSectionTitle(
                            column,
                            "APPROVAL");

                        column.Item()
                            .Text(
                                $"Approved : {(report.IsApproved ? "YES" : "NO")}\n" +
                                $"Approved By : {GetValue(report.ApprovedBy)}")
                            .FontSize(8);

                        column.Item()
                            .Text(
                                $"Inspection Date : {report.InspectionDate:dd-MMM-yyyy HH:mm}\n" +
                                $"Generated Date  : {report.GeneratedDate:dd-MMM-yyyy HH:mm}")
                            .FontSize(8);
                    });

                AddFooter(page);
            });
        })
        .GeneratePdf(filePath);

        return filePath;
    }

    private static void AddFooter(
        PageDescriptor page)
    {
        page.Footer()
            .AlignCenter()
            .Column(footer =>
            {
                footer.Item()
                    .LineHorizontal(0.5f);

                footer.Item()
                    .Text(
                        "PS SuperNDT Platform - Confidential Inspection Report")
                    .FontSize(7);

                footer.Item()
                    .Text(text =>
                    {
                        text.Span("Page ")
                            .FontSize(7);

                        text.CurrentPageNumber();

                        text.Span(" of ")
                            .FontSize(7);

                        text.TotalPages();
                    });
            });
    }

    private static void AddSectionTitle(
        ColumnDescriptor column,
        string title)
    {
        column.Item()
            .PaddingTop(3)
            .Text(title)
            .FontSize(10)
            .Bold();
    }

    private static void AddInformationTable(
        ColumnDescriptor column,
        ReportDataModel report)
    {
        column.Item()
            .Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(105);
                    columns.RelativeColumn();
                    columns.ConstantColumn(105);
                    columns.RelativeColumn();
                });

                AddTableRow(
                    table,
                    "Report Number",
                    report.ReportNumber,
                    "Inspection Date",
                    report.InspectionDate.ToString(
                        "dd-MMM-yyyy"));

                AddTableRow(
                    table,
                    "Customer",
                    report.Customer,
                    "Project",
                    report.Project);

                AddTableRow(
                    table,
                    "Component",
                    report.Component,
                    "Weld Number",
                    report.WeldNumber);

                AddTableRow(
                    table,
                    "Operator",
                    report.Operator,
                    "Inspector",
                    report.Inspector);
            });
    }

    private static void AddInspectionDetails(
        ColumnDescriptor column,
        ReportDataModel report)
    {
        column.Item()
            .Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(105);
                    columns.RelativeColumn();
                    columns.ConstantColumn(105);
                    columns.RelativeColumn();
                });

                AddTableRow(
                    table,
                    "Procedure",
                    report.Procedure,
                    "Material",
                    report.Material);

                AddTableRow(
                    table,
                    "Technique",
                    report.Technique,
                    "Job ID",
                    report.JobId.ToString());
            });
    }

    private static void AddTableRow(
        TableDescriptor table,
        string label1,
        string value1,
        string label2,
        string value2)
    {
        table.Cell()
            .Border(0.5f)
            .Padding(3)
            .Text(label1)
            .Bold()
            .FontSize(7);

        table.Cell()
            .Border(0.5f)
            .Padding(3)
            .Text(GetValue(value1))
            .FontSize(7);

        table.Cell()
            .Border(0.5f)
            .Padding(3)
            .Text(label2)
            .Bold()
            .FontSize(7);

        table.Cell()
            .Border(0.5f)
            .Padding(3)
            .Text(GetValue(value2))
            .FontSize(7);
    }

    private static void AddFindingsTable(
        ColumnDescriptor column,
        ReportDataModel report)
    {
        if (report.Findings == null ||
            report.Findings.Count == 0)
        {
            column.Item()
                .Text("No findings recorded.")
                .FontSize(8);

            return;
        }

        column.Item()
            .Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(28);
                    columns.RelativeColumn(1.2f);
                    columns.RelativeColumn(1.2f);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(1.5f);
                });

                AddFindingHeader(
                    table,
                    "No.",
                    "Location",
                    "Type",
                    "Description",
                    "Severity",
                    "Evaluation");

                foreach (var finding in report.Findings.OrderBy(
                             x => x.FindingNumber))
                {
                    table.Cell()
                        .Border(0.5f)
                        .Padding(2)
                        .Text(finding.FindingNumber.ToString())
                        .FontSize(6);

                    table.Cell()
                        .Border(0.5f)
                        .Padding(2)
                        .Text(GetValue(finding.Location))
                        .FontSize(6);

                    table.Cell()
                        .Border(0.5f)
                        .Padding(2)
                        .Text(GetValue(finding.FindingType))
                        .FontSize(6);

                    table.Cell()
                        .Border(0.5f)
                        .Padding(2)
                        .Text(GetValue(finding.Description))
                        .FontSize(6);

                    table.Cell()
                        .Border(0.5f)
                        .Padding(2)
                        .Text(GetValue(finding.Severity))
                        .FontSize(6);

                    string evaluation =
                        string.IsNullOrWhiteSpace(
                            finding.Evaluation)
                            ? finding.IsAccepted
                                ? "ACCEPTED"
                                : "NOT ACCEPTED"
                            : finding.Evaluation;

                    table.Cell()
                        .Border(0.5f)
                        .Padding(2)
                        .Text(evaluation)
                        .FontSize(6);
                }
            });
    }

    private static void AddFindingHeader(
        TableDescriptor table,
        string number,
        string location,
        string type,
        string description,
        string severity,
        string evaluation)
    {
        AddHeaderCell(table, number);
        AddHeaderCell(table, location);
        AddHeaderCell(table, type);
        AddHeaderCell(table, description);
        AddHeaderCell(table, severity);
        AddHeaderCell(table, evaluation);
    }

    private static void AddHeaderCell(
        TableDescriptor table,
        string text)
    {
        table.Cell()
            .Border(0.5f)
            .Padding(2)
            .Text(text)
            .Bold()
            .FontSize(6);
    }

    private static void AddImages(
        ColumnDescriptor column,
        ReportDataModel report)
    {
        List<ReportImageModel> validImages =
            report.Images
                .Where(image =>
                    !string.IsNullOrWhiteSpace(
                        GetExistingImagePath(image)))
                .OrderBy(image => image.SequenceNumber)
                .ToList();

        if (validImages.Count == 0)
        {
            column.Item()
                .Text("No reviewed image files were found.")
                .FontSize(8);

            return;
        }

        column.Item()
            .Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                foreach (ReportImageModel image in validImages)
                {
                    string imagePath =
                        GetExistingImagePath(image);

                    table.Cell()
                        .Border(0.5f)
                        .Padding(3)
                        .Column(cell =>
                        {
                            cell.Spacing(2);

                            cell.Item()
                                .Text(
                                    $"SHOT {image.SequenceNumber}" +
                                    (string.IsNullOrWhiteSpace(
                                        image.ImageName)
                                        ? string.Empty
                                        : $" - {image.ImageName}"))
                                .Bold()
                                .FontSize(7);

                            cell.Item()
                                .Height(145)
                                .Image(imagePath);

                            string caption =
                                BuildImageCaption(image);

                            if (!string.IsNullOrWhiteSpace(
                                    caption))
                            {
                                cell.Item()
                                    .Text(caption)
                                    .FontSize(6)
                                    .LineHeight(1.0f);
                            }
                        });
                }
            });
    }

    private static string BuildImageCaption(
        ReportImageModel image)
    {
        List<string> parts =
            new();

        if (!string.IsNullOrWhiteSpace(
                image.Description))
        {
            parts.Add(
                image.Description.Trim());
        }

        if (!string.IsNullOrWhiteSpace(
                image.Remarks))
        {
            parts.Add(
                $"Remarks: {image.Remarks.Trim()}");
        }

        return string.Join(
            " | ",
            parts);
    }

    private static string GetExistingImagePath(
        ReportImageModel image)
    {
        if (!string.IsNullOrWhiteSpace(
                image.FilePath) &&
            File.Exists(image.FilePath))
        {
            return image.FilePath;
        }

        if (!string.IsNullOrWhiteSpace(
                image.ThumbnailPath) &&
            File.Exists(image.ThumbnailPath))
        {
            return image.ThumbnailPath;
        }

        return string.Empty;
    }

    private static string GetValue(
        string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? "-"
            : value;
    }

    private static string CreateReportFilePath(
        string? reportNumber)
    {
        string reportsFolder =
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.MyDocuments),
                "PS SuperNDT Reports");

        Directory.CreateDirectory(
            reportsFolder);

        string safeReportNumber =
            SanitizeFileName(reportNumber);

        string fileName =
            $"Report_{safeReportNumber}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

        return Path.Combine(
            reportsFolder,
            fileName);
    }

    private static string SanitizeFileName(
        string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "InspectionReport";
        }

        foreach (char invalidCharacter in
                 Path.GetInvalidFileNameChars())
        {
            value = value.Replace(
                invalidCharacter,
                '_');
        }

        return value;
    }
}