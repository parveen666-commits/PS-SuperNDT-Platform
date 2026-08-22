using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReviewedImageExportService
{
    private readonly ImageFolderService _imageFolderService;

    public ReviewedImageExportService()
    {
        _imageFolderService =
            new ImageFolderService();
    }

    /// <summary>
    /// Creates a reviewed PNG copy of the supplied image.
    ///
    /// The original image is never modified.
    /// Defect rectangles are permanently rendered into the
    /// exported reviewed image.
    /// </summary>
    public string ExportReviewedPng(
        ImageRecordModel image,
        BitmapSource source,
        IEnumerable<DefectModel>? defects = null)
    {
        if (image == null)
        {
            throw new ArgumentNullException(
                nameof(image));
        }

        if (source == null)
        {
            throw new ArgumentNullException(
                nameof(source));
        }

        string status =
            NormalizeReviewStatus(
                image.ReviewStatus);

        string destinationFolder =
            _imageFolderService.GetStatusFolder(
                image.JobNumber,
                status);

        string originalBaseName =
            BuildBaseName(
                image);

        string destinationPath =
            Path.Combine(
                destinationFolder,
                $"{originalBaseName}_REVIEWED.png");

        destinationPath =
            BuildUniqueDestinationPath(
                destinationPath);

        int pixelWidth =
            source.PixelWidth;

        int pixelHeight =
            source.PixelHeight;

        if (pixelWidth <= 0 ||
            pixelHeight <= 0)
        {
            throw new InvalidOperationException(
                "The selected image has an invalid size.");
        }

        DrawingVisual visual =
            new DrawingVisual();

        using (DrawingContext drawing =
               visual.RenderOpen())
        {
            drawing.DrawRectangle(
                Brushes.Black,
                null,
                new Rect(
                    0,
                    0,
                    pixelWidth,
                    pixelHeight));

            drawing.DrawImage(
                source,
                new Rect(
                    0,
                    0,
                    pixelWidth,
                    pixelHeight));

            if (defects != null)
            {
                foreach (
                    DefectModel defect in defects)
                {
                    DrawDefect(
                        drawing,
                        defect,
                        pixelWidth,
                        pixelHeight);
                }
            }
        }

        RenderTargetBitmap renderedImage =
            new RenderTargetBitmap(
                pixelWidth,
                pixelHeight,
                96,
                96,
                PixelFormats.Pbgra32);

        renderedImage.Render(
            visual);

        renderedImage.Freeze();

        PngBitmapEncoder encoder =
            new PngBitmapEncoder();

        encoder.Frames.Add(
            BitmapFrame.Create(
                renderedImage));

        Directory.CreateDirectory(
            destinationFolder);

        using (
            FileStream stream =
                new FileStream(
                    destinationPath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None))
        {
            encoder.Save(stream);
        }

        return destinationPath;
    }

    private static void DrawDefect(
        DrawingContext drawing,
        DefectModel defect,
        int imageWidth,
        int imageHeight)
    {
        double x =
            Math.Clamp(
                defect.X,
                0,
                imageWidth);

        double y =
            Math.Clamp(
                defect.Y,
                0,
                imageHeight);

        double width =
            Math.Max(
                1,
                defect.Width);

        double height =
            Math.Max(
                1,
                defect.Height);

        if (x + width >
            imageWidth)
        {
            width =
                imageWidth - x;
        }

        if (y + height >
            imageHeight)
        {
            height =
                imageHeight - y;
        }

        if (width <= 0 ||
            height <= 0)
        {
            return;
        }

        Rect rectangle =
            new Rect(
                x,
                y,
                width,
                height);

        Pen defectPen =
            new Pen(
                new SolidColorBrush(
                    Color.FromRgb(
                        255,
                        60,
                        60)),
                3);

        defectPen.Brush.Freeze();

        drawing.DrawRectangle(
            new SolidColorBrush(
                Color.FromArgb(
                    45,
                    255,
                    60,
                    60)),
            defectPen,
            rectangle);

        string defectType =
            string.IsNullOrWhiteSpace(
                defect.DefectType)
                ? "UNCLASSIFIED"
                : defect.DefectType.Trim();

        string severity =
            string.IsNullOrWhiteSpace(
                defect.Severity)
                ? "UNCLASSIFIED"
                : defect.Severity.Trim();

        string label =
            $"{defectType} | {severity}";

        if (string.IsNullOrWhiteSpace(
                defect.Description) == false)
        {
            label +=
                $" | {defect.Description.Trim()}";
        }

        FormattedText text =
            new FormattedText(
                label,
                System.Globalization.CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new Typeface(
                    new FontFamily("Segoe UI"),
                    FontStyles.Normal,
                    FontWeights.Bold,
                    FontStretches.Normal),
                12,
                Brushes.White,
                1.0);

        double labelX =
            x;

        double labelY =
            Math.Max(
                0,
                y - text.Height - 4);

        Rect labelBackground =
            new Rect(
                labelX,
                labelY,
                text.Width + 8,
                text.Height + 4);

        drawing.DrawRectangle(
            new SolidColorBrush(
                Color.FromArgb(
                    210,
                    20,
                    24,
                    31)),
            null,
            labelBackground);

        drawing.DrawText(
            text,
            new Point(
                labelX + 4,
                labelY + 2));
    }

    private static string NormalizeReviewStatus(
        string? status)
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

        return "REPAIR";
    }

    private static string BuildBaseName(
        ImageRecordModel image)
    {
        string originalName =
            string.IsNullOrWhiteSpace(
                image.FileName)
                ? $"SHOT_{image.ShotNumber:000}"
                : Path.GetFileName(
                    image.FileName);

        string baseName =
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
            $"{baseName}_S{image.ShotNumber:000}_{image.Id:N}";
    }

    private static string BuildUniqueDestinationPath(
        string destination)
    {
        if (!File.Exists(destination))
        {
            return destination;
        }

        string directory =
            Path.GetDirectoryName(
                destination) ?? string.Empty;

        string baseName =
            Path.GetFileNameWithoutExtension(
                destination);

        string extension =
            Path.GetExtension(
                destination);

        int counter = 1;

        string candidate;

        do
        {
            candidate =
                Path.Combine(
                    directory,
                    $"{baseName}_{counter}{extension}");

            counter++;

        } while (File.Exists(candidate));

        return candidate;
    }

    private static string SanitizeFileName(
        string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "IMAGE";
        }

        char[] invalid =
            Path.GetInvalidFileNameChars();

        char[] chars =
            value.Trim().ToCharArray();

        for (int i = 0;
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

        string result =
            new string(chars).Trim();

        return string.IsNullOrWhiteSpace(result)
            ? "IMAGE"
            : result;
    }
}