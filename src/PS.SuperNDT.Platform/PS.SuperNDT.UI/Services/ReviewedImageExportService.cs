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
        _imageFolderService =
            new ImageFolderService();
    }

    public string ExportReviewedPng(
        ImageRecordModel image,
        BitmapSource source,
        IEnumerable<DefectModel>? defects = null)
    {
        ArgumentNullException.ThrowIfNull(image);
        ArgumentNullException.ThrowIfNull(source);

        if (string.IsNullOrWhiteSpace(
                image.FilePath))
        {
            throw new InvalidOperationException(
                "The selected image file path is empty.");
        }

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

        string sourcePath =
            image.FilePath;

        string? directory =
            Path.GetDirectoryName(
                sourcePath);

        if (string.IsNullOrWhiteSpace(
                directory))
        {
            throw new InvalidOperationException(
                "The selected image destination folder is invalid.");
        }

        Directory.CreateDirectory(
            directory);

        /*
         * IMPORTANT
         *
         * NEVER overwrite the original inspection image.
         *
         * The original image must remain clean because ReviewView
         * draws the live defect overlay on top of it.
         *
         * If the original PNG is overwritten with the reviewed
         * annotation, opening Review again will show:
         *
         *     1. old burned-in defect
         *     2. new live defect
         *
         * which creates the extra defect box seen in Review.
         */
        string destinationPath =
            BuildReviewedFilePath(
                sourcePath);

        BitmapSource normalizedSource =
            NormalizeToPixelBitmap(
                source);

        DrawingVisual visual =
            new DrawingVisual();

        using (
            DrawingContext drawing =
                visual.RenderOpen())
        {
            drawing.DrawImage(
                normalizedSource,
                new Rect(
                    0,
                    0,
                    pixelWidth,
                    pixelHeight));

            if (defects != null)
            {
                foreach (
                    DefectModel defect
                    in defects)
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

        string temporaryPath =
            destinationPath +
            ".tmp";

        try
        {
            PngBitmapEncoder encoder =
                new PngBitmapEncoder();

            encoder.Frames.Add(
                BitmapFrame.Create(
                    renderedImage));

            using (
                FileStream stream =
                    new FileStream(
                        temporaryPath,
                        FileMode.Create,
                        FileAccess.Write,
                        FileShare.None))
            {
                encoder.Save(stream);
            }

            ReplaceFile(
                temporaryPath,
                destinationPath);
        }
        finally
        {
            try
            {
                if (File.Exists(
                        temporaryPath))
                {
                    File.Delete(
                        temporaryPath);
                }
            }
            catch
            {
                // Cleanup failure must not hide
                // a successful export.
            }
        }

        return destinationPath;
    }

    private static string BuildReviewedFilePath(
        string sourcePath)
    {
        string directory =
            Path.GetDirectoryName(
                sourcePath)
            ?? string.Empty;

        string fileName =
            Path.GetFileNameWithoutExtension(
                sourcePath);

        return Path.Combine(
            directory,
            fileName +
            "_REVIEWED.png");
    }

    private static BitmapSource NormalizeToPixelBitmap(
        BitmapSource source)
    {
        int width =
            source.PixelWidth;

        int height =
            source.PixelHeight;

        FormatConvertedBitmap converted =
            new FormatConvertedBitmap(
                source,
                PixelFormats.Pbgra32,
                null,
                0);

        converted.Freeze();

        WriteableBitmap bitmap =
            new WriteableBitmap(
                width,
                height,
                96,
                96,
                PixelFormats.Pbgra32,
                null);

        int stride =
            width * 4;

        byte[] pixels =
            new byte[
                stride *
                height];

        converted.CopyPixels(
            pixels,
            stride,
            0);

        bitmap.WritePixels(
            new Int32Rect(
                0,
                0,
                width,
                height),
            pixels,
            stride,
            0);

        bitmap.Freeze();

        return bitmap;
    }

    private static void DrawDefect(
        DrawingContext drawing,
        DefectModel defect,
        int imageWidth,
        int imageHeight)
    {
        /*
         * DefectModel geometry is stored in SOURCE IMAGE PIXELS.
         *
         * Export surface is also created at SOURCE IMAGE PIXELS.
         *
         * Therefore no Review zoom, viewport size or DIP conversion
         * is performed here.
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
                    60,
                    60));

        SolidColorBrush border =
            new SolidColorBrush(
                Color.FromRgb(
                    255,
                    60,
                    60));

        Pen pen =
            new Pen(
                border,
                2);

        fill.Freeze();
        border.Freeze();
        pen.Freeze();

        drawing.DrawRectangle(
            fill,
            pen,
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
        string type =
            string.IsNullOrWhiteSpace(
                defect.DefectType)
                ? "UNCLASSIFIED"
                : defect.DefectType.Trim();

        string severity =
            string.IsNullOrWhiteSpace(
                defect.Severity)
                ? "UNCLASSIFIED"
                : defect.Severity.Trim();

        string position =
            $"POS       {defect.PipePosition:0.0} mm";

        string length =
            $"LENGTH    {defect.LengthMm:0.0} mm";

        string width =
            $"WIDTH     {defect.WidthMm:0.0} mm";

        string severityText =
            $"SEVERITY  {severity}";

        string? remark =
            string.IsNullOrWhiteSpace(
                defect.Description)
                ? null
                : $"REMARK    {defect.Description.Trim()}";

        const double cardWidth = 255;
        const double padding = 8;
        const double fontSize = 10;
        const double lineHeight = 13;

        double contentHeight =
            13 +
            lineHeight +
            lineHeight +
            lineHeight +
            lineHeight;

        if (!string.IsNullOrWhiteSpace(
                remark))
        {
            contentHeight +=
                CalculateWrappedTextHeight(
                    remark,
                    cardWidth -
                    (padding * 2),
                    fontSize);
        }

        double cardHeight =
            contentHeight +
            (padding * 2);

        double center =
            defectRect.Left +
            (defectRect.Width / 2.0);

        double cardLeft =
            center -
            (cardWidth / 2.0);

        cardLeft =
            Math.Clamp(
                cardLeft,
                5,
                Math.Max(
                    5,
                    imageWidth -
                    cardWidth -
                    5));

        double cardTop =
            defectRect.Top -
            cardHeight -
            8;

        if (cardTop < 5)
        {
            cardTop =
                defectRect.Bottom +
                8;
        }

        if (cardTop + cardHeight >
            imageHeight - 5)
        {
            cardTop =
                Math.Max(
                    5,
                    imageHeight -
                    cardHeight -
                    5);
        }

        Rect cardRect =
            new Rect(
                cardLeft,
                cardTop,
                cardWidth,
                cardHeight);

        SolidColorBrush background =
            new SolidColorBrush(
                Color.FromArgb(
                    245,
                    20,
                    24,
                    31));

        SolidColorBrush borderBrush =
            new SolidColorBrush(
                Color.FromRgb(
                    255,
                    75,
                    75));

        Pen borderPen =
            new Pen(
                borderBrush,
                1);

        background.Freeze();
        borderBrush.Freeze();
        borderPen.Freeze();

        drawing.DrawRoundedRectangle(
            background,
            borderPen,
            cardRect,
            4,
            4);

        double textX =
            cardLeft +
            padding;

        double textY =
            cardTop +
            padding;

        DrawInfoText(
            drawing,
            $"DEFECT  •  {type}",
            textX,
            ref textY,
            cardWidth -
            (padding * 2),
            true);

        DrawInfoText(
            drawing,
            position,
            textX,
            ref textY,
            cardWidth -
            (padding * 2));

        DrawInfoText(
            drawing,
            length,
            textX,
            ref textY,
            cardWidth -
            (padding * 2));

        DrawInfoText(
            drawing,
            width,
            textX,
            ref textY,
            cardWidth -
            (padding * 2));

        DrawInfoText(
            drawing,
            severityText,
            textX,
            ref textY,
            cardWidth -
            (padding * 2));

        if (!string.IsNullOrWhiteSpace(
                remark))
        {
            DrawInfoText(
                drawing,
                remark,
                textX,
                ref textY,
                cardWidth -
                (padding * 2));
        }
    }

    private static void DrawInfoText(
        DrawingContext drawing,
        string text,
        double x,
        ref double y,
        double maxWidth,
        bool bold = false)
    {
        Typeface typeface =
            new Typeface(
                new FontFamily("Segoe UI"),
                FontStyles.Normal,
                bold
                    ? FontWeights.Bold
                    : FontWeights.Normal,
                FontStretches.Normal);

        FormattedText formattedText =
            new FormattedText(
                text,
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                typeface,
                10,
                Brushes.White,
                1.0);

        formattedText.MaxTextWidth =
            maxWidth;

        formattedText.Trimming =
            TextTrimming.CharacterEllipsis;

        drawing.DrawText(
            formattedText,
            new Point(
                x,
                y));

        y +=
            Math.Max(
                13,
                formattedText.Height);
    }

    private static double CalculateWrappedTextHeight(
        string text,
        double width,
        double fontSize)
    {
        FormattedText formattedText =
            new FormattedText(
                text,
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new Typeface(
                    new FontFamily("Segoe UI"),
                    FontStyles.Normal,
                    FontWeights.Normal,
                    FontStretches.Normal),
                fontSize,
                Brushes.White,
                1.0);

        formattedText.MaxTextWidth =
            width;

        formattedText.Trimming =
            TextTrimming.CharacterEllipsis;

        return Math.Max(
            13,
            formattedText.Height);
    }

    private static void ReplaceFile(
        string temporaryPath,
        string destinationPath)
    {
        if (File.Exists(
                destinationPath))
        {
            try
            {
                File.Replace(
                    temporaryPath,
                    destinationPath,
                    null);

                return;
            }
            catch
            {
                try
                {
                    File.Delete(
                        destinationPath);
                }
                catch
                {
                    // If the old reviewed file is locked,
                    // use a new unique file name below.
                    string uniquePath =
                        BuildUniqueReviewedPath(
                            destinationPath);

                    File.Move(
                        temporaryPath,
                        uniquePath);

                    return;
                }
            }
        }

        File.Move(
            temporaryPath,
            destinationPath);
    }

    private static string BuildUniqueReviewedPath(
        string destinationPath)
    {
        string directory =
            Path.GetDirectoryName(
                destinationPath)
            ?? string.Empty;

        string fileName =
            Path.GetFileNameWithoutExtension(
                destinationPath);

        int counter = 2;

        while (true)
        {
            string candidate =
                Path.Combine(
                    directory,
                    $"{fileName}_{counter}.png");

            if (!File.Exists(candidate))
            {
                return candidate;
            }

            counter++;
        }
    }
}