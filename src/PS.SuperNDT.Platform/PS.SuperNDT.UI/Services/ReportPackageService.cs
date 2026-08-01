using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportPackageService
{
    public bool CreatePackage(
        string sourceFolder,
        string destinationZipFile,
        out ReportPackageModel package)
    {
        package = new ReportPackageModel();

        if (!Directory.Exists(sourceFolder))
            return false;

        try
        {
            if (File.Exists(destinationZipFile))
                File.Delete(destinationZipFile);

            ZipFile.CreateFromDirectory(
                sourceFolder,
                destinationZipFile,
                CompressionLevel.Optimal,
                false);

            FileInfo fileInfo = new(destinationZipFile);

            package = new ReportPackageModel
            {
                Id = Guid.NewGuid(),
                PackageNumber = $"PKG-{DateTime.Now:yyyyMMddHHmmss}",
                PackagePath = destinationZipFile,
                FileName = Path.GetFileName(destinationZipFile),
                PackageSizeBytes = fileInfo.Length,
                FileCount = Directory.GetFiles(sourceFolder, "*.*", SearchOption.AllDirectories).Length,
                IsCompressed = true,
                IsVerified = File.Exists(destinationZipFile),
                CreatedOn = DateTime.Now,
                CreatedBy = Environment.UserName,
                Remarks = "Package created successfully."
            };

            return true;
        }
        catch
        {
            package = new ReportPackageModel();
            return false;
        }
    }

    public bool ExtractPackage(string zipFile, string destinationFolder)
    {
        if (!File.Exists(zipFile))
            return false;

        try
        {
            if (Directory.Exists(destinationFolder))
                Directory.Delete(destinationFolder, true);

            ZipFile.ExtractToDirectory(zipFile, destinationFolder);

            return true;
        }
        catch
        {
            return false;
        }
    }
}