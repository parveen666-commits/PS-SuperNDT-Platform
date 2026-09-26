using PS.SuperNDT.UI.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PS.SuperNDT.UI.Services;

public sealed class ReviewedImageExportService
{
    private const string ReviewedFolderName = "Reviewed";

    private const double MinimumBoxSize = 8.0;

    private const double TextPadding = 4.0;

    private const double TextFontSize = 14.0;

    private const double LabelGap = 3.0;

    public string ExportReviewedPng(
        ImageRecordModel image,
        BitmapSource displayImage,
        IEnumerable defects)
    {
        if (image == null)
        {
            throw new ArgumentNullException(nameof(image));
        }

        if (displayImage == null)
        {
            throw new ArgumentNullException(nameof(displayImage));
        }

        if (displayImage.PixelWidth <= 0 ||
            displayImage.PixelHeight <= 0)
        {
            throw new InvalidOperationException(
                "The image has an invalid size.");
        }

        string sourcePath =
            image.FilePath ?? string.Empty;

        string sourceDirectory =
            !string.IsNullOrWhiteSpace(sourcePath)
                ? Path.GetDirectoryName(sourcePath) ?? string.Empty
                : string.Empty;

        if (string.IsNullOrWhiteSpace(sourceDirectory))
        {
            sourceDirectory =
                Path.Combine(
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.LocalApplicationData),
                    "PS.SuperNDT",
                    "Reviewed");
        }

        string reviewedDirectory =
            Path.Combine(
                sourceDirectory,
                ReviewedFolderName);

        Directory.CreateDirectory(
            reviewedDirectory);

        string sourceFileName =
            !string.IsNullOrWhiteSpace(sourcePath)
                ? Path.GetFileNameWithoutExtension(sourcePath)
                : $"Shot_{image.ShotNumber}";

        if (string.IsNullOrWhiteSpace(sourceFileName))
        {
            sourceFileName =
                $"Shot_{image.ShotNumber}";
        }

        string outputFileName =
            $"{sourceFileName}_REVIEWED.png";

        string outputPath =
            Path.Combine(
                reviewedDirectory,
                outputFileName);

        return ExportToPath(
            displayImage,
            defects,
            outputPath);
    }

    private static string ExportToPath(
        BitmapSource source,
        IEnumerable defects,
        string outputPath)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (string.IsNullOrWhiteSpace(outputPath))
        {
            throw new ArgumentException(
                "Output path is empty.",
                nameof(outputPath));
        }

        string? directory =
            Path.GetDirectoryName(outputPath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var defectList =
            MaterializeDefects(defects);

        int width =
            source.PixelWidth;

        int height =
            source.PixelHeight;

        var visual =
            new DrawingVisual();

        using (DrawingContext drawingContext =
               visual.RenderOpen())
        {
            drawingContext.DrawImage(
                source,
                new Rect(
                    0,
                    0,
                    width,
                    height));

            foreach (object defect in defectList)
            {
                DrawDefect(
                    drawingContext,
                    defect,
                    width,
                    height);
            }
        }

        var renderedBitmap =
            new RenderTargetBitmap(
                width,
                height,
                source.DpiX > 0
                    ? source.DpiX
                    : 96,
                source.DpiY > 0
                    ? source.DpiY
                    : 96,
                PixelFormats.Pbgra32);

        renderedBitmap.Render(
            visual);

        renderedBitmap.Freeze();

        var encoder =
            new PngBitmapEncoder();

        encoder.Frames.Add(
            BitmapFrame.Create(
                renderedBitmap));

        string temporaryPath =
            outputPath +
            "." +
            Guid.NewGuid().ToString("N") +
            ".tmp";

        try
        {
            using (var stream =
                   new FileStream(
                       temporaryPath,
                       FileMode.Create,
                       FileAccess.Write,
                       FileShare.None))
            {
                encoder.Save(stream);
                stream.Flush(true);
            }

            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }

            File.Move(
                temporaryPath,
                outputPath);
        }
        finally
        {
            try
            {
                if (File.Exists(temporaryPath))
                {
                    File.Delete(temporaryPath);
                }
            }
            catch
            {
                // Do not hide the original export exception.
            }
        }

        return outputPath;
    }

    private static List<object> MaterializeDefects(
        IEnumerable defects)
    {
        var result =
            new List<object>();

        if (defects == null)
        {
            return result;
        }

        foreach (object? defect in defects)
        {
            if (defect != null)
            {
                result.Add(defect);
            }
        }

        return result;
    }

    private static void DrawDefect(
        DrawingContext drawingContext,
        object defect,
        double imageWidth,
        double imageHeight)
    {
        if (defect == null)
        {
            return;
        }

        if (!TryGetRectangle(
                defect,
                imageWidth,
                imageHeight,
                out Rect rectangle))
        {
            return;
        }

        rectangle =
            NormalizeRectangle(
                rectangle,
                imageWidth,
                imageHeight);

        if (rectangle.Width < MinimumBoxSize)
        {
            rectangle.Width =
                Math.Min(
                    MinimumBoxSize,
                    imageWidth -
                    rectangle.X);
        }

        if (rectangle.Height < MinimumBoxSize)
        {
            rectangle.Height =
                Math.Min(
                    MinimumBoxSize,
                    imageHeight -
                    rectangle.Y);
        }

        if (rectangle.Width <= 0 ||
            rectangle.Height <= 0)
        {
            return;
        }

        DrawDefectBox(
            drawingContext,
            rectangle);

        string label =
            BuildDefectLabel(
                defect);

        if (!string.IsNullOrWhiteSpace(label))
        {
            DrawDefectLabel(
                drawingContext,
                rectangle,
                label,
                imageWidth,
                imageHeight);
        }
    }

    private static Rect NormalizeRectangle(
        Rect rectangle,
        double imageWidth,
        double imageHeight)
    {
        double x =
            Math.Max(
                0,
                Math.Min(
                    rectangle.X,
                    imageWidth));

        double y =
            Math.Max(
                0,
                Math.Min(
                    rectangle.Y,
                    imageHeight));

        double right =
            Math.Max(
                x,
                Math.Min(
                    rectangle.Right,
                    imageWidth));

        double bottom =
            Math.Max(
                y,
                Math.Min(
                    rectangle.Bottom,
                    imageHeight));

        return new Rect(
            x,
            y,
            Math.Max(
                0,
                right - x),
            Math.Max(
                0,
                bottom - y));
    }

    private static void DrawDefectBox(
        DrawingContext drawingContext,
        Rect rectangle)
    {
        var pen =
            new Pen(
                Brushes.Red,
                3.0);

        pen.Freeze();

        drawingContext.DrawRectangle(
            null,
            pen,
            rectangle);

        double cornerSize =
            Math.Min(
                12.0,
                Math.Min(
                    rectangle.Width,
                    rectangle.Height) /
                3.0);

        if (cornerSize <= 0)
        {
            return;
        }

        var cornerPen =
            new Pen(
                Brushes.Red,
                5.0);

        cornerPen.Freeze();

        double left =
            rectangle.Left;

        double top =
            rectangle.Top;

        double right =
            rectangle.Right;

        double bottom =
            rectangle.Bottom;

        DrawCorner(
            drawingContext,
            cornerPen,
            left,
            top,
            cornerSize,
            true,
            true);

        DrawCorner(
            drawingContext,
            cornerPen,
            right,
            top,
            cornerSize,
            false,
            true);

        DrawCorner(
            drawingContext,
            cornerPen,
            left,
            bottom,
            cornerSize,
            true,
            false);

        DrawCorner(
            drawingContext,
            cornerPen,
            right,
            bottom,
            cornerSize,
            false,
            false);
    }

    private static void DrawCorner(
        DrawingContext drawingContext,
        Pen pen,
        double x,
        double y,
        double size,
        bool left,
        bool top)
    {
        double horizontalDirection =
            left ? 1 : -1;

        double verticalDirection =
            top ? 1 : -1;

        drawingContext.DrawLine(
            pen,
            new Point(
                x,
                y),
            new Point(
                x +
                horizontalDirection *
                size,
                y));

        drawingContext.DrawLine(
            pen,
            new Point(
                x,
                y),
            new Point(
                x,
                y +
                verticalDirection *
                size));
    }

    private static void DrawDefectLabel(
        DrawingContext drawingContext,
        Rect rectangle,
        string label,
        double imageWidth,
        double imageHeight)
    {
        var typeface =
            new Typeface(
                new FontFamily("Segoe UI"),
                FontStyles.Normal,
                FontWeights.SemiBold,
                FontStretches.Normal);

        var formattedText =
            new FormattedText(
                label,
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                typeface,
                TextFontSize,
                Brushes.White,
                1.0);

        formattedText.MaxTextWidth =
            Math.Max(
                80,
                imageWidth -
                TextPadding * 2);

        double labelWidth =
            formattedText.Width +
            TextPadding * 2;

        double labelHeight =
            formattedText.Height +
            TextPadding * 2;

        double labelX =
            rectangle.Left;

        double labelY =
            rectangle.Top -
            labelHeight -
            LabelGap;

        if (labelY < 0)
        {
            labelY =
                rectangle.Bottom +
                LabelGap;
        }

        if (labelY + labelHeight >
            imageHeight)
        {
            labelY =
                Math.Max(
                    0,
                    imageHeight -
                    labelHeight);
        }

        if (labelX + labelWidth >
            imageWidth)
        {
            labelX =
                Math.Max(
                    0,
                    imageWidth -
                    labelWidth);
        }

        var background =
            new SolidColorBrush(
                Color.FromArgb(
                    220,
                    120,
                    0,
                    0));

        background.Freeze();

        var border =
            new Pen(
                Brushes.Red,
                1.5);

        border.Freeze();

        var labelRect =
            new Rect(
                labelX,
                labelY,
                Math.Min(
                    labelWidth,
                    imageWidth -
                    labelX),
                Math.Min(
                    labelHeight,
                    imageHeight -
                    labelY));

        drawingContext.DrawRectangle(
            background,
            border,
            labelRect);

        drawingContext.DrawText(
            formattedText,
            new Point(
                labelRect.Left +
                TextPadding,
                labelRect.Top +
                TextPadding));
    }

    private static string BuildDefectLabel(
        object defect)
    {
        string defectType =
            GetString(
                defect,
                "DefectType",
                "Type",
                "Defect");

        string severity =
            GetString(
                defect,
                "Severity");

        string position =
            GetString(
                defect,
                "Position",
                "PositionText");

        string length =
            GetString(
                defect,
                "Length",
                "DefectLength");

        string width =
            GetString(
                defect,
                "Width",
                "DefectWidth");

        var parts =
            new List<string>();

        if (!string.IsNullOrWhiteSpace(defectType))
        {
            parts.Add(
                defectType.Trim());
        }

        if (!string.IsNullOrWhiteSpace(severity))
        {
            parts.Add(
                $"Severity: {severity.Trim()}");
        }

        if (!string.IsNullOrWhiteSpace(position))
        {
            parts.Add(
                $"Pos: {position.Trim()}");
        }

        if (!string.IsNullOrWhiteSpace(length))
        {
            parts.Add(
                $"L: {length.Trim()}");
        }

        if (!string.IsNullOrWhiteSpace(width))
        {
            parts.Add(
                $"W: {width.Trim()}");
        }

        return string.Join(
            " | ",
            parts);
    }

    private static string GetString(
        object source,
        params string[] propertyNames)
    {
        foreach (string propertyName in propertyNames)
        {
            PropertyInfo? property =
                source.GetType()
                    .GetProperty(
                        propertyName,
                        BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.IgnoreCase);

            if (property == null)
            {
                continue;
            }

            object? value;

            try
            {
                value =
                    property.GetValue(source);
            }
            catch
            {
                continue;
            }

            if (value == null)
            {
                continue;
            }

            string text =
                Convert.ToString(
                    value,
                    CultureInfo.InvariantCulture)
                ?? string.Empty;

            if (!string.IsNullOrWhiteSpace(text))
            {
                return text;
            }
        }

        return string.Empty;
    }

    private static bool TryGetRectangle(
        object defect,
        double imageWidth,
        double imageHeight,
        out Rect rectangle)
    {
        rectangle = Rect.Empty;

        /*
         * First try a Rect-like property.
         */
        string[] rectangleProperties =
        {
            "Bounds",
            "Rectangle",
            "Rect"
        };

        foreach (string propertyName
                 in rectangleProperties)
        {
            if (TryGetPropertyValue(
                    defect,
                    propertyName,
                    out object? value) &&
                value != null &&
                TryConvertToRect(
                    value,
                    out rectangle))
            {
                return true;
            }
        }

        /*
         * Try X/Y/Width/Height.
         */
        if (TryGetDouble(
                defect,
                out double x,
                "X",
                "Left",
                "StartX"))
        {
            if (TryGetDouble(
                    defect,
                    out double y,
                    "Y",
                    "Top",
                    "StartY"))
            {
                if (TryGetDouble(
                        defect,
                        out double width,
                        "Width",
                        "BoxWidth",
                        "DefectWidth"))
                {
                    if (TryGetDouble(
                            defect,
                            out double height,
                            "Height",
                            "BoxHeight",
                            "DefectHeight"))
                    {
                        rectangle =
                            new Rect(
                                x,
                                y,
                                width,
                                height);

                        return true;
                    }
                }
            }
        }

        /*
         * Try StartX/StartY/EndX/EndY.
         */
        if (TryGetDouble(
                defect,
                out double startX,
                "StartX",
                "X1"))
        {
            if (TryGetDouble(
                    defect,
                    out double startY,
                    "StartY",
                    "Y1"))
            {
                if (TryGetDouble(
                        defect,
                        out double endX,
                        "EndX",
                        "X2"))
                {
                    if (TryGetDouble(
                            defect,
                            out double endY,
                            "EndY",
                            "Y2"))
                    {
                        rectangle =
                            new Rect(
                                new Point(
                                    startX,
                                    startY),
                                new Point(
                                    endX,
                                    endY));

                        return true;
                    }
                }
            }
        }

        /*
         * Try Position + Size style properties.
         */
        if (TryGetPoint(
                defect,
                out Point position,
                "Position",
                "Location"))
        {
            if (TryGetSize(
                    defect,
                    out Size size,
                    "Size"))
            {
                rectangle =
                    new Rect(
                        position,
                        size);

                return true;
            }
        }

        /*
         * Last fallback:
         * if the defect exposes normalized coordinates,
         * convert them to actual image pixels.
         */
        if (TryGetDouble(
                defect,
                out double normalizedX,
                "NormalizedX",
                "RelativeX"))
        {
            if (TryGetDouble(
                    defect,
                    out double normalizedY,
                    "NormalizedY",
                    "RelativeY"))
            {
                if (TryGetDouble(
                        defect,
                        out double normalizedWidth,
                        "NormalizedWidth",
                        "RelativeWidth"))
                {
                    if (TryGetDouble(
                            defect,
                            out double normalizedHeight,
                            "NormalizedHeight",
                            "RelativeHeight"))
                    {
                        rectangle =
                            new Rect(
                                normalizedX *
                                imageWidth,
                                normalizedY *
                                imageHeight,
                                normalizedWidth *
                                imageWidth,
                                normalizedHeight *
                                imageHeight);

                        return true;
                    }
                }
            }
        }

        return false;
    }

    private static bool TryConvertToRect(
        object value,
        out Rect rectangle)
    {
        rectangle = Rect.Empty;

        if (value is Rect rect)
        {
            rectangle = rect;
            return true;
        }

        Type type =
            value.GetType();

        if (TryReadDouble(
                value,
                type,
                "X",
                out double x) &&
            TryReadDouble(
                value,
                type,
                "Y",
                out double y) &&
            TryReadDouble(
                value,
                type,
                "Width",
                out double width) &&
            TryReadDouble(
                value,
                type,
                "Height",
                out double height))
        {
            rectangle =
                new Rect(
                    x,
                    y,
                    width,
                    height);

            return true;
        }

        return false;
    }

    private static bool TryGetPoint(
        object source,
        out Point point,
        params string[] propertyNames)
    {
        point = new Point();

        foreach (string propertyName
                 in propertyNames)
        {
            if (!TryGetPropertyValue(
                    source,
                    propertyName,
                    out object? value) ||
                value == null)
            {
                continue;
            }

            if (value is Point directPoint)
            {
                point = directPoint;
                return true;
            }

            Type type =
                value.GetType();

            if (TryReadDouble(
                    value,
                    type,
                    "X",
                    out double x) &&
                TryReadDouble(
                    value,
                    type,
                    "Y",
                    out double y))
            {
                point =
                    new Point(
                        x,
                        y);

                return true;
            }
        }

        return false;
    }

    private static bool TryGetSize(
        object source,
        out Size size,
        params string[] propertyNames)
    {
        size = new Size();

        foreach (string propertyName
                 in propertyNames)
        {
            if (!TryGetPropertyValue(
                    source,
                    propertyName,
                    out object? value) ||
                value == null)
            {
                continue;
            }

            if (value is Size directSize)
            {
                size = directSize;
                return true;
            }

            Type type =
                value.GetType();

            if (TryReadDouble(
                    value,
                    type,
                    "Width",
                    out double width) &&
                TryReadDouble(
                    value,
                    type,
                    "Height",
                    out double height))
            {
                size =
                    new Size(
                        width,
                        height);

                return true;
            }
        }

        return false;
    }

    private static bool TryGetDouble(
        object source,
        out double value,
        params string[] propertyNames)
    {
        value = 0;

        foreach (string propertyName
                 in propertyNames)
        {
            if (TryGetPropertyValue(
                    source,
                    propertyName,
                    out object? propertyValue) &&
                propertyValue != null &&
                TryConvertDouble(
                    propertyValue,
                    out value))
            {
                return true;
            }
        }

        return false;
    }

    private static bool TryReadDouble(
        object source,
        Type type,
        string propertyName,
        out double value)
    {
        value = 0;

        PropertyInfo? property =
            type.GetProperty(
                propertyName,
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.IgnoreCase);

        if (property == null)
        {
            return false;
        }

        object? propertyValue;

        try
        {
            propertyValue =
                property.GetValue(source);
        }
        catch
        {
            return false;
        }

        return propertyValue != null &&
               TryConvertDouble(
                   propertyValue,
                   out value);
    }

    private static bool TryGetPropertyValue(
        object source,
        string propertyName,
        out object? value)
    {
        value = null;

        PropertyInfo? property =
            source.GetType()
                .GetProperty(
                    propertyName,
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.IgnoreCase);

        if (property == null)
        {
            return false;
        }

        try
        {
            value =
                property.GetValue(source);

            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool TryConvertDouble(
        object value,
        out double result)
    {
        if (value is double doubleValue)
        {
            result = doubleValue;
            return true;
        }

        if (value is float floatValue)
        {
            result = floatValue;
            return true;
        }

        if (value is int intValue)
        {
            result = intValue;
            return true;
        }

        if (value is long longValue)
        {
            result = longValue;
            return true;
        }

        if (value is decimal decimalValue)
        {
            result =
                (double)decimalValue;

            return true;
        }

        return double.TryParse(
            Convert.ToString(
                value,
                CultureInfo.InvariantCulture),
            NumberStyles.Float |
            NumberStyles.AllowThousands,
            CultureInfo.InvariantCulture,
            out result);
    }
}