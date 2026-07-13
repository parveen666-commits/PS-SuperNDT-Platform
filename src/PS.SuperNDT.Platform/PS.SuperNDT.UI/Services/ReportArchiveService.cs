using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportArchiveService
{
    public ReportArchiveModel Archive(
        Guid reportId,
        string reportNumber,
        string filePath,
        string archivedBy)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException(
                "Report file path cannot be empty.",
                nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException(
                "Report file not found.",
                filePath);
        }

        return new ReportArchiveModel
        {
            ReportId = reportId,
            ReportNumber = reportNumber,
            FilePath = filePath,
            FileHash = CalculateHash(filePath),
            ArchivedBy = archivedBy,
            ArchivedOn = DateTime.Now,
            Version = "1.0",
            IsLocked = true
        };
    }

    private static string CalculateHash(string filePath)
    {
        using var sha256 = SHA256.Create();

        byte[] bytes = File.ReadAllBytes(filePath);
        byte[] hash = sha256.ComputeHash(bytes);

        var builder = new StringBuilder();

        foreach (byte item in hash)
        {
            builder.Append(item.ToString("x2"));
        }

        return builder.ToString();
    }
}