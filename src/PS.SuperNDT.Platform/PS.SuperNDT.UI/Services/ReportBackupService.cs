using System;
using System.IO;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportBackupService
{
    private readonly string _backupFolder;

    public ReportBackupService()
    {
        _backupFolder = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.MyDocuments),
            "PS SuperNDT",
            "ReportBackups");

        Directory.CreateDirectory(_backupFolder);
    }

    public string Backup(
        string sourceFile)
    {
        if (string.IsNullOrWhiteSpace(sourceFile))
        {
            throw new ArgumentException(
                "Source file is required.",
                nameof(sourceFile));
        }

        if (!File.Exists(sourceFile))
        {
            throw new FileNotFoundException(
                "Report file not found.",
                sourceFile);
        }

        string destinationFile = Path.Combine(
            _backupFolder,
            $"{DateTime.Now:yyyyMMdd_HHmmss}_{Path.GetFileName(sourceFile)}");

        File.Copy(
            sourceFile,
            destinationFile,
            true);

        return destinationFile;
    }

    public string GetBackupFolder()
    {
        return _backupFolder;
    }
}