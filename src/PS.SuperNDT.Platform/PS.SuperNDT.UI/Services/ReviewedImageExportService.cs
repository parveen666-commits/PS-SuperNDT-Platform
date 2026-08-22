using System;
using System.Collections.Generic;
using System.Globalization;
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
        _imageFolderService = new ImageFolderService();
    }

    public string ExportReviewedPng(
        ImageRecordModel image,
        BitmapSource source,
        IEnumerable<DefectModel>? defects = null)
    {
        if (image == null)
        {
            throw new ArgumentNullException(nameof(image));
        }

        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (string.IsNullOrWhiteSpace(image.FilePath))
        {
            throw new InvalidOperationException(
                "The selected image does not have a valid file path.");
        }

        string destinationPath =
            image.FilePath;

        string? destinationDirectory =
            Path.GetDirectoryName(destinationPath);

        if (string.IsNullOrWhiteSpace(destinationDirectory))
        {
            throw new InvalidOperationException(
                "The selected image path is invalid.");
        }

        Directory.CreateDirectory(
            destinationDirectory);

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

        /*
         * IMPORTANT
         *
         * The export surface is created using the SOURCE IMAGE
         * pixel dimensions.
         *
         * Defect X/Y/Width/Height are stored in source-image
         * pixels by ReviewView.
         *
         * Therefore:
         *
         *     DB pixel coordinates
         *              ↓
         *     PNG pixel coordinates
         *
         * No ZoomLevel
         * No Canvas coordinates
         * No ShotFrame coordinates
         * No WPF mouse/DIP coordinates
         */

        DrawingVisual visual =
            new DrawingVisual();

        using (DrawingContext drawing =
               visual.RenderOpen())
        {
            drawing.DrawImage(
                source,
                new Rect(
                    0,
                    0,
                    pixelWidth,
                    pixelHeight));

            if (defects != null)
            {
                foreach (DefectModel defect in defects)
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

        /*
         * Write to a temporary file first.
         *
         * This prevents the original PNG from being damaged
         * if encoding or file replacement fails.
         */

        string temporaryPath =
            Path.Combine(
                destinationDirectory,
                $".{Path.GetFileName(destinationPath)}.{Guid.NewGuid():N}.tmp");

        try
        {
            using (FileStream stream =
                   new FileStream(
                       temporaryPath,
                       FileMode.CreateNew,
                       FileAccess.Write,
                       FileShare.None))
            {
                encoder.Save(stream);
                stream.Flush(true);
            }

            /*
             * Replace the existing PNG with the reviewed PNG.
             *
             * Final result:
             *
             *     ONE PNG
             *
             * The old unmarked PNG is removed.
             */

            ReplaceFile(
                temporaryPath,
                destinationPath);
        }
        catch
        {
            TryDeleteFile(
                temporaryPath);

            throw;
        }

        return destinationPath;
    }

    private static void ReplaceFile(
        string temporaryPath,
        string destinationPath)
    {
        if (File.Exists(destinationPath))
        {
            File.Delete(
                destinationPath);
        }

        File.Move(
            temporaryPath,
            destinationPath);
    }

    private static void DrawDefect(
        DrawingContext drawing,
        DefectModel defect,
        int imageWidth,
        int imageHeight)
    {
        /*
         * Defect coordinates are SOURCE IMAGE PIXELS.
         */

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

        if (x + width > imageWidth)
        {
            width =
                imageWidth - x;
        }

        if (y + height > imageHeight)
        {
            height =
                imageHeight - y;
        }

        if (width <= 0 ||
            height <= 0)
        {
            return;
        }

        Rect defectRect =
            new Rect(
                x,
                y,
                width,
                height);

        SolidColorBrush fill =
            new SolidColorBrush(
                Color.FromArgb(
                    35,
                    255,
                    0,
                    0));

        SolidColorBrush borderBrush =
            new SolidColorBrush(
                Color.FromRgb(
                    255,
                    40,
                    40));

        Pen borderPen =
            new Pen(
                borderBrush,
                3);

        fill.Freeze();
        borderBrush.Freeze();
        borderPen.Freeze();

        drawing.DrawRectangle(
            fill,
            borderPen,
            defectRect);

        DrawDefectDetailCard(
            drawing,
            defect,
            defectRect,
            imageWidth,
            imageHeight);
    }

    private static void DrawDefectDetailCard(
        DrawingContext drawing,
        DefectModel defect,
        Rect defectRect,
        int imageWidth,
        int imageHeight)
    {
        const double cardWidth = 330;
        const double cardPadding = 10;

        string type =
            SafeText(
                defect.DefectType,
                "UNCLASSIFIED");

        string severity =
            SafeText(
                defect.Severity,
                "UNCLASSIFIED");

        string description =
            SafeText(
                defect.Description,
                "No description");

        string status =
            SafeText(
                defect.Status,
                "OPEN");

        string thicknessStatus =
            SafeText(
                defect.ThicknessStatus,
                "NOT CHECKED");

        string thicknessRemark =
            SafeText(
                defect.ThicknessRemark,
                "-");

        string createdBy =
            SafeText(
                defect.CreatedBy,
                "-");

        List<string> lines =
            new List<string>
            {
                $"DEFECT: {type}",
                $"SEVERITY: {severity}",
                $"STATUS: {status}",
                $"DESCRIPTION: {description}",
                "",
                $"LENGTH: {FormatNumber(defect.LengthMm)} mm",
                $"WIDTH: {FormatNumber(defect.WidthMm)} mm",
                "",
                $"PIPE POSITION: {FormatNumber(defect.PipePosition)} mm",
                $"PIPE LENGTH: {FormatNumber(defect.PipeLength)} mm",
                $"SHOT START: {FormatNumber(defect.ShotStartPosition)} mm",
                $"SHOT END: {FormatNumber(defect.ShotEndPosition)} mm",
                "",
                $"NOMINAL THK: {FormatNumber(defect.NominalThicknessMm)} mm",
                $"ACTUAL THK: {FormatNumber(defect.ActualThicknessMm)} mm",
                $"MIN THK: {FormatNumber(defect.MinimumThicknessMm)} mm",
                $"THICKNESS: {thicknessStatus}",
                $"THK REMARK: {thicknessRemark}",
                "",
                $"CREATED BY: {createdBy}",
                $"CREATED: {defect.CreatedOn:dd-MM-yyyy HH:mm}"
            };

        const double fontSize = 11;
        const double lineHeight = 17;

        double cardHeight =
            cardPadding * 2 +
            lines.Count * lineHeight;

        double cardX =
            defectRect.Right + 12;

        double cardY =
            defectRect.Top;

        /*
         * Right side unavailable:
         * place card on left.
         */

        if (cardX + cardWidth >
            imageWidth)
        {
            cardX =
                defectRect.Left -
                cardWidth -
                12;
        }

        /*
         * If left side also unavailable,
         * keep card inside image.
         */

        if (cardX < 0)
        {
            cardX = 4;
        }

        /*
         * Keep card vertically inside image.
         */

        if (cardY + cardHeight >
            imageHeight)
        {
            cardY =
                imageHeight -
                cardHeight -
                4;
        }

        if (cardY < 0)
        {
            cardY = 4;
        }

        Rect cardRect =
            new Rect(
                cardX,
                cardY,
                cardWidth,
                cardHeight);

        SolidColorBrush background =
            new SolidColorBrush(
                Color.FromArgb(
                    225,
                    20,
                    24,
                    31));

        SolidColorBrush border =
            new SolidColorBrush(
                Color.FromRgb(
                    255,
                    70,
                    70));

        Pen cardPen =
            new Pen(
                border,
                2);

        background.Freeze();
        border.Freeze();
        cardPen.Freeze();

        drawing.DrawRoundedRectangle(
            background,
            cardPen,
            cardRect,
            6,
            6);

        double textY =
            cardY + cardPadding;

        for (int i = 0;
             i < lines.Count;
             i++)
        {
            string line =
                lines[i];

            if (string.IsNullOrWhiteSpace(line))
            {
                textY += lineHeight;
                continue;
            }

            FontWeight weight =
                i <= 2
                    ? FontWeights.Bold
                    : FontWeights.Normal;

            SolidColorBrush textBrush =
                i == 1
                    ? new SolidColorBrush(
                        Color.FromRgb(
                            255,
                            170,
                            70))
                    : Brushes.White;

            FormattedText text =
                new FormattedText(
                    line,
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(
                        new FontFamily("Segoe UI"),
                        FontStyles.Normal,
                        weight,
                        FontStretches.Normal),
                    fontSize,
                    textBrush,
                    1.0);

            drawing.DrawText(
                text,
                new Point(
                    cardX + cardPadding,
                    textY));

            textY += lineHeight;
        }
    }

    private static string SafeText(
        string? value,
        string fallback)
    {
        return string.IsNullOrWhiteSpace(value)
            ? fallback
            : value.Trim();
    }

    private static string FormatNumber(
        double value)
    {
        return value.ToString(
            "0.##",
            CultureInfo.InvariantCulture);
    }

    private static void TryDeleteFile(
        string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch
        {
            // Do not hide the original export exception.
        }
    }
}