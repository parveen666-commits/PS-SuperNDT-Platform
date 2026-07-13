using System;
using System.IO;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportStorageService
{
    private readonly string _storagePath;

    public ReportStorageService()
    {
        _storagePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "PS SuperNDT",
            "Reports");

        Directory.CreateDirectory(_storagePath);
    }

    public string Save(
        ReportDataModel report,
        string content)
    {
        if (report == null)
        {
            throw new ArgumentNullException(nameof(report));
        }

        string fileName =
            $"{report.ReportNumber}.txt";

        string filePath =
            Path.Combine(_storagePath, fileName);

        File.WriteAllText(
            filePath,
            content);

        return filePath;
    }

    public bool Exists(
        string reportNumber)
    {
        string filePath =
            Path.Combine(
                _storagePath,
                $"{reportNumber}.txt");

        return File.Exists(filePath);
    }
}