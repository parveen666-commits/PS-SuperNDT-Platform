using System;
using System.IO;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ImageFolderService
{
    private readonly string _jobsRoot;

    public ImageFolderService()
    {
        _jobsRoot =
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Jobs");

        Directory.CreateDirectory(_jobsRoot);
    }

    public string GetJobFolder(
        string jobNumber)
    {
        var safeJobNumber =
            SanitizeFolderName(jobNumber);

        var jobFolder =
            Path.Combine(
                _jobsRoot,
                safeJobNumber);

        Directory.CreateDirectory(jobFolder);

        return jobFolder;
    }

    public string GetStatusFolder(
        string jobNumber,
        string status)
    {
        var jobFolder =
            GetJobFolder(jobNumber);

        var folderName =
            NormalizeStatusFolder(status);

        var statusFolder =
            Path.Combine(
                jobFolder,
                folderName);

        Directory.CreateDirectory(statusFolder);

        return statusFolder;
    }

    public string GetImagePath(
        ImageRecordModel image,
        string status)
    {
        if (image == null)
        {
            throw new ArgumentNullException(
                nameof(image));
        }

        var jobNumber =
            string.IsNullOrWhiteSpace(
                image.JobNumber)
                ? "UNKNOWN_JOB"
                : image.JobNumber;

        var folder =
            GetStatusFolder(
                jobNumber,
                status);

        var fileName =
            BuildFileName(image);

        return Path.Combine(
            folder,
            fileName);
    }

    public string MoveImageToStatus(
        ImageRecordModel image,
        string status)
    {
        if (image == null)
        {
            throw new ArgumentNullException(
                nameof(image));
        }

        if (string.IsNullOrWhiteSpace(
                image.FilePath))
        {
            return string.Empty;
        }

        if (!File.Exists(
                image.FilePath))
        {
            return image.FilePath;
        }

        var destination =
            GetImagePath(
                image,
                status);

        if (PathsEqual(
                image.FilePath,
                destination))
        {
            return destination;
        }

        Directory.CreateDirectory(
            Path.GetDirectoryName(
                destination)!);

        if (File.Exists(destination))
        {
            destination =
                BuildUniqueDestinationPath(
                    destination);
        }

        File.Move(
            image.FilePath,
            destination);

        return destination;
    }

    public string CopyImageToStatus(
        ImageRecordModel image,
        string status)
    {
        if (image == null)
        {
            throw new ArgumentNullException(
                nameof(image));
        }

        if (string.IsNullOrWhiteSpace(
                image.FilePath))
        {
            return string.Empty;
        }

        if (!File.Exists(
                image.FilePath))
        {
            return image.FilePath;
        }

        var destination =
            GetImagePath(
                image,
                status);

        Directory.CreateDirectory(
            Path.GetDirectoryName(
                destination)!);

        if (File.Exists(destination))
        {
            destination =
                BuildUniqueDestinationPath(
                    destination);
        }

        File.Copy(
            image.FilePath,
            destination);

        return destination;
    }

    public string GetRootFolder()
    {
        Directory.CreateDirectory(
            _jobsRoot);

        return _jobsRoot;
    }

    private static string NormalizeStatusFolder(
        string status)
    {
        if (string.Equals(
                status,
                "ACCEPTED",
                StringComparison.OrdinalIgnoreCase))
        {
            return "ACCEPT";
        }

        if (string.Equals(
                status,
                "ACCEPT",
                StringComparison.OrdinalIgnoreCase))
        {
            return "ACCEPT";
        }

        if (string.Equals(
                status,
                "REJECTED",
                StringComparison.OrdinalIgnoreCase))
        {
            return "REJECT";
        }

        if (string.Equals(
                status,
                "REJECT",
                StringComparison.OrdinalIgnoreCase))
        {
            return "REJECT";
        }

        if (string.Equals(
                status,
                "REPAIR",
                StringComparison.OrdinalIgnoreCase))
        {
            return "REPAIR";
        }

        return "REPAIR";
    }

    private static string BuildFileName(
        ImageRecordModel image)
    {
        var originalName =
            string.IsNullOrWhiteSpace(
                image.FileName)
                ? $"SHOT_{image.ShotNumber:000}"
                : Path.GetFileName(
                    image.FileName);

        var extension =
            Path.GetExtension(
                originalName);

        if (string.IsNullOrWhiteSpace(
                extension))
        {
            extension = ".png";
        }

        var baseName =
            Path.GetFileNameWithoutExtension(
                originalName);

        if (string.IsNullOrWhiteSpace(
                baseName))
        {
            baseName =
                $"SHOT_{image.ShotNumber:000}";
        }

        baseName =
            SanitizeFileName(
                baseName);

        return
            $"{baseName}_S{image.ShotNumber:000}_{image.Id:N}{extension}";
    }

    private static string BuildUniqueDestinationPath(
        string destination)
    {
        var directory =
            Path.GetDirectoryName(
                destination)!;

        var fileName =
            Path.GetFileNameWithoutExtension(
                destination);

        var extension =
            Path.GetExtension(
                destination);

        var counter = 1;

        string candidate;

        do
        {
            candidate =
                Path.Combine(
                    directory,
                    $"{fileName}_{counter}{extension}");

            counter++;

        } while (File.Exists(candidate));

        return candidate;
    }

    private static bool PathsEqual(
        string first,
        string second)
    {
        var firstFullPath =
            Path.GetFullPath(first)
                .TrimEnd(
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar);

        var secondFullPath =
            Path.GetFullPath(second)
                .TrimEnd(
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar);

        return string.Equals(
            firstFullPath,
            secondFullPath,
            StringComparison.OrdinalIgnoreCase);
    }

    private static string SanitizeFolderName(
        string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "UNKNOWN_JOB";
        }

        var invalid =
            Path.GetInvalidFileNameChars();

        var chars =
            value.Trim()
                .ToCharArray();

        for (var i = 0;
             i < chars.Length;
             i++)
        {
            if (Array.IndexOf(
                    invalid,
                    chars[i]) >= 0)
            {
                chars[i] = '_';
            }
        }

        var result =
            new string(chars).Trim();

        return string.IsNullOrWhiteSpace(result)
            ? "UNKNOWN_JOB"
            : result;
    }

    private static string SanitizeFileName(
        string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "IMAGE";
        }

        var invalid =
            Path.GetInvalidFileNameChars();

        var chars =
            value.Trim()
                .ToCharArray();

        for (var i = 0;
             i < chars.Length;
             i++)
        {
            if (Array.IndexOf(
                    invalid,
                    chars[i]) >= 0)
            {
                chars[i] = '_';
            }
        }

        var result =
            new string(chars).Trim();

        return string.IsNullOrWhiteSpace(result)
            ? "IMAGE"
            : result;
    }
}