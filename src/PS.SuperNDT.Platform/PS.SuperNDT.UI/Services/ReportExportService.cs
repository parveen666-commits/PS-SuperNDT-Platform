using System;
using System.IO;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportExportService
{
    private readonly ReportGeneratorService _reportGeneratorService;
    private readonly PdfExportService _pdfExportService;

    public ReportExportService()
    {
        _reportGeneratorService =
            new ReportGeneratorService();

        _pdfExportService =
            new PdfExportService();
    }


    public string ExportReport(
        ReportDataModel report)
    {
        ArgumentNullException.ThrowIfNull(report);


        string content =
            _reportGeneratorService
                .GenerateReportSummary(report);


        string filePath =
            _pdfExportService.Export(
                content,
                report.ReportNumber);


        return filePath;
    }


    public bool Exists(
        string filePath)
    {
        return File.Exists(filePath);
    }
}