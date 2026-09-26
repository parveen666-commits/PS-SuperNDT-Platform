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

    // ============================================================
    // JOB FOLDER
    // ============================================================

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

    // ============================================================
    // STATUS FOLDER
    // ============================================================

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

    // ============================================================
    // IMAGE PATH
    // ============================================================

    public string GetImagePath(
        ImageRecordModel image,
        string status)
    {
        ArgumentNullException.ThrowIfNull(image);

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

    // ============================================================
    // MOVE IMAGE
    // ============================================================

    public string MoveImageToStatus(
        ImageRecordModel image,
        string status)
    {
        ArgumentNullException.ThrowIfNull(image);

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
            CleanupOtherStatusCopies(
                image,
                destination);

            return destination;
        }

        Directory.CreateDirectory(
            Path.GetDirectoryName(
                destination)!);

        /*
         * Remove any old copy of the same image
         * from PENDING / ACCEPT / REJECT / REPAIR.
         *
         * This guarantees that one image exists
         * in only one status folder.
         */
        CleanupOtherStatusCopies(
            image,
            destination);

        if (File.Exists(destination))
        {
            try
            {
                File.Delete(destination);
            }
            catch
            {
                destination =
                    BuildUniqueDestinationPath(
                        destination);
            }
        }

        File.Move(
            image.FilePath,
            destination);

        return destination;
    }

    // ============================================================
    // COPY IMAGE
    // ============================================================

    public string CopyImageToStatus(
        ImageRecordModel image,
        string status)
    {
        ArgumentNullException.ThrowIfNull(image);

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

        /*
         * IMPORTANT:
         *
         * Even when caller uses COPY instead of MOVE,
         * remove all previous status copies first.
         *
         * Otherwise:
         *
         * PENDING
         * ACCEPT
         * REJECT
         *
         * can all contain the same image.
         */
        CleanupOtherStatusCopies(
            image,
            destination);

        if (File.Exists(destination))
        {
            try
            {
                File.Delete(destination);
            }
            catch
            {
                destination =
                    BuildUniqueDestinationPath(
                        destination);
            }
        }

        File.Copy(
            image.FilePath,
            destination);

        return destination;
    }

    // ============================================================
    // CLEAN OLD STATUS COPIES
    // ============================================================

    private void CleanupOtherStatusCopies(
        ImageRecordModel image,
        string keepPath)
    {
        var jobNumber =
            string.IsNullOrWhiteSpace(
                image.JobNumber)
                ? "UNKNOWN_JOB"
                : image.JobNumber;

        var jobFolder =
            GetJobFolder(jobNumber);

        string[] statusFolders =
        {
            "PENDING",
            "ACCEPT",
            "REJECT",
            "REPAIR"
        };

        var expectedFileName =
            BuildFileName(image);

        foreach (var statusFolderName
                 in statusFolders)
        {
            var folder =
                Path.Combine(
                    jobFolder,
                    statusFolderName);

            if (!Directory.Exists(folder))
            {
                continue;
            }

            var exactPath =
                Path.Combine(
                    folder,
                    expectedFileName);

            if (PathsEqual(
                    exactPath,
                    keepPath))
            {
                continue;
            }

            try
            {
                if (File.Exists(exactPath))
                {
                    File.Delete(exactPath);
                }
            }
            catch
            {
                /*
                 * Do not break status change because
                 * an old copy is temporarily locked.
                 */
            }

            /*
             * Also remove any generated duplicate names
             * such as:
             *
             * IMAGE_S001_ID_1.png
             * IMAGE_S001_ID_2.png
             *
             * belonging to this same image.
             */
            try
            {
                var baseName =
                    Path.GetFileNameWithoutExtension(
                        expectedFileName);

                var extension =
                    Path.GetExtension(
                        expectedFileName);

                foreach (var file
                         in Directory.GetFiles(
                             folder,
                             baseName + "_*" + extension))
                {
                    if (PathsEqual(
                            file,
                            keepPath))
                    {
                        continue;
                    }

                    try
                    {
                        File.Delete(file);
                    }
                    catch
                    {
                        // Ignore locked stale copies.
                    }
                }
            }
            catch
            {
                // Ignore folder cleanup errors.
            }
        }
    }

    // ============================================================
    // ROOT
    // ============================================================

    public string GetRootFolder()
    {
        Directory.CreateDirectory(
            _jobsRoot);

        return _jobsRoot;
    }

    // ============================================================
    // STATUS NORMALIZATION
    // ============================================================

    private static string NormalizeStatusFolder(
        string status)
    {
        if (string.Equals(
                status,
                "PENDING",
                StringComparison.OrdinalIgnoreCase))
        {
            return "PENDING";
        }

        if (string.Equals(
                status,
                "ACCEPTED",
                StringComparison.OrdinalIgnoreCase) ||
            string.Equals(
                status,
                "ACCEPT",
                StringComparison.OrdinalIgnoreCase))
        {
            return "ACCEPT";
        }

        if (string.Equals(
                status,
                "REJECTED",
                StringComparison.OrdinalIgnoreCase) ||
            string.Equals(
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

        /*
         * Unknown status should never silently become
         * REPAIR.
         *
         * Keep it in PENDING.
         */
        return "PENDING";
    }

    // ============================================================
    // FILE NAME
    // ============================================================

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

    // ============================================================
    // UNIQUE DESTINATION
    // ============================================================

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

        }
        while (File.Exists(candidate));

        return candidate;
    }

    // ============================================================
    // PATH COMPARISON
    // ============================================================

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

    // ============================================================
    // FOLDER NAME
    // ============================================================

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

    // ============================================================
    // FILE NAME SANITIZATION
    // ============================================================

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