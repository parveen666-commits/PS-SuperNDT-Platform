using System;
using System.IO;
using System.Text;

namespace PS.SuperNDT.UI.Services;

public sealed class PdfExportService
{
    public string Export(string reportContent, string reportNumber)
    {
        if (string.IsNullOrWhiteSpace(reportContent))
        {
            throw new ArgumentException("Report content cannot be empty.", nameof(reportContent));
        }

        string reportsFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "PS SuperNDT Reports");

        Directory.CreateDirectory(reportsFolder);

        string fileName =
            $"Report_{reportNumber}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

        string filePath = Path.Combine(reportsFolder, fileName);

        // Temporary PDF generation placeholder.
        // Real AERB formatted PDF engine will be integrated in next phase.

        var pdfContent = new StringBuilder();

        pdfContent.AppendLine("PS SuperNDT Platform");
        pdfContent.AppendLine("AERB Style Inspection Report");
        pdfContent.AppendLine("--------------------------------");
        pdfContent.AppendLine();
        pdfContent.AppendLine(reportContent);

        File.WriteAllText(filePath, pdfContent.ToString());

        return filePath;
    }
}