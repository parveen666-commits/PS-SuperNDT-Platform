using System;
using System.IO;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportExportService
{
    private readonly PdfExportService _pdfExportService;

    public ReportExportService()
    {
        _pdfExportService =
            new PdfExportService();
    }

    public string ExportReport(
        ReportDataModel report)
    {
        ArgumentNullException.ThrowIfNull(report);

        string filePath =
            _pdfExportService.Export(
                report);

        return filePath;
    }

    public bool Exists(
        string filePath)
    {
        return File.Exists(filePath);
    }
}