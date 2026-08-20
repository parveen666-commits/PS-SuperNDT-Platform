using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PS.SuperNDT.UI.Services;

public sealed class DrImageOverlayService
{
    public void ApplyOverlay(
        string imagePath,
        string pipeId,
        int shotNumber,
        int totalShots,
        double startPositionMm,
        double endPositionMm,
        bool rulerEnabled = true,
        bool pipeIdOverlayEnabled = true)
    {
        if (string.IsNullOrWhiteSpace(imagePath) ||
            !File.Exists(imagePath))
        {
            throw new FileNotFoundException(
                "DR image was not found.",
                imagePath);
        }

        var source = new BitmapImage();

        source.BeginInit();
        source.UriSource =
            new Uri(
                imagePath,
                UriKind.Absolute);
        source.CacheOption =
            BitmapCacheOption.OnLoad;
        source.EndInit();
        source.Freeze();

        int width =
            source.PixelWidth;

        int height =
            source.PixelHeight;

        var visual =
            new DrawingVisual();

        using (DrawingContext dc =
               visual.RenderOpen())
        {
            dc.DrawImage(
                source,
                new Rect(
                    0,
                    0,
                    width,
                    height));

            DrawHeader(
                dc,
                width,
                pipeId,
                shotNumber,
                totalShots,
                startPositionMm,
                endPositionMm,
                pipeIdOverlayEnabled);

            if (rulerEnabled)
            {
                DrawRuler(
                    dc,
                    width,
                    height,
                    startPositionMm,
                    endPositionMm);
            }
        }

        var rendered =
            new RenderTargetBitmap(
                width,
                height,
                96,
                96,
                PixelFormats.Pbgra32);

        rendered.Render(visual);
        rendered.Freeze();

        var encoder =
            new PngBitmapEncoder();

        encoder.Frames.Add(
            BitmapFrame.Create(rendered));

        string temporaryPath =
            imagePath + ".overlay.tmp";

        using (FileStream stream =
               new(
                   temporaryPath,
                   FileMode.Create,
                   FileAccess.Write,
                   FileShare.None))
        {
            encoder.Save(stream);
        }

        File.Copy(
            temporaryPath,
            imagePath,
            true);

        File.Delete(
            temporaryPath);
    }

    private static void DrawHeader(
        DrawingContext dc,
        int width,
        string pipeId,
        int shotNumber,
        int totalShots,
        double startPositionMm,
        double endPositionMm,
        bool pipeIdOverlayEnabled)
    {
        const double margin = 18;
        const double top = 18;

        string shotText =
            $"SHOT {shotNumber}/{totalShots}";

        string positionText =
            $"{startPositionMm:0} - {endPositionMm:0} mm";

        string pipeText =
            $"PIPE ID: {pipeId}";

        var titleTypeface =
            new Typeface(
                new FontFamily("Segoe UI"),
                FontStyles.Normal,
                FontWeights.Bold,
                FontStretches.Normal);

        var normalTypeface =
            new Typeface(
                new FontFamily("Segoe UI"),
                FontStyles.Normal,
                FontWeights.Normal,
                FontStretches.Normal);

        var titleBrush =
            Brushes.White;

        var secondaryBrush =
            Brushes.LightGray;

        const double shotFontSize = 22;
        const double normalFontSize = 17;

        var shotFormatted =
            new FormattedText(
                shotText,
                System.Globalization.CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                titleTypeface,
                shotFontSize,
                titleBrush,
                1.0);

        var positionFormatted =
            new FormattedText(
                positionText,
                System.Globalization.CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                normalTypeface,
                normalFontSize,
                secondaryBrush,
                1.0);

        double boxWidth =
            Math.Min(
                width - (margin * 2),
                Math.Max(
                    shotFormatted.Width,
                    positionFormatted.Width) + 36);

        if (pipeIdOverlayEnabled)
        {
            var pipeFormatted =
                new FormattedText(
                    pipeText,
                    System.Globalization.CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    titleTypeface,
                    normalFontSize,
                    Brushes.Cyan,
                    1.0);

            boxWidth =
                Math.Min(
                    width - (margin * 2),
                    Math.Max(
                        boxWidth,
                        pipeFormatted.Width + 36));
        }

        double boxHeight =
            pipeIdOverlayEnabled
                ? 92
                : 66;

        var background =
            new SolidColorBrush(
                Color.FromArgb(
                    210,
                    20,
                    25,
                    30));

        background.Freeze();

        var borderBrush =
            new SolidColorBrush(
                Color.FromArgb(
                    230,
                    0,
                    229,
                    255));

        borderBrush.Freeze();

        var rectangle =
            new Rect(
                margin,
                top,
                boxWidth,
                boxHeight);

        dc.DrawRoundedRectangle(
            background,
            new Pen(
                borderBrush,
                1.5),
            rectangle,
            6,
            6);

        double textX =
            margin + 14;

        double textY =
            top + 8;

        if (pipeIdOverlayEnabled)
        {
            var pipeFormatted =
                new FormattedText(
                    pipeText,
                    System.Globalization.CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    titleTypeface,
                    normalFontSize,
                    Brushes.Cyan,
                    1.0);

            dc.DrawText(
                pipeFormatted,
                new Point(
                    textX,
                    textY));

            textY += 25;
        }

        dc.DrawText(
            shotFormatted,
            new Point(
                textX,
                textY));

        textY += 28;

        dc.DrawText(
            positionFormatted,
            new Point(
                textX,
                textY));
    }

    private static void DrawRuler(
        DrawingContext dc,
        int width,
        int height,
        double startPositionMm,
        double endPositionMm)
    {
        if (endPositionMm <= startPositionMm)
        {
            return;
        }

        double left = 55;
        double right = width - 55;
        double rulerY = height - 72;
        double rulerWidth = right - left;

        var rulerBrush =
            new SolidColorBrush(
                Color.FromArgb(
                    235,
                    255,
                    255,
                    255));

        rulerBrush.Freeze();

        var majorBrush =
            new SolidColorBrush(
                Color.FromArgb(
                    245,
                    0,
                    229,
                    255));

        majorBrush.Freeze();

        var rulerPen =
            new Pen(
                rulerBrush,
                2);

        var majorPen =
            new Pen(
                majorBrush,
                2);

        dc.DrawLine(
            rulerPen,
            new Point(
                left,
                rulerY),
            new Point(
                right,
                rulerY));

        double range =
            endPositionMm -
            startPositionMm;

        double tickStep =
            CalculateTickStep(range);

        double firstTick =
            Math.Ceiling(
                startPositionMm /
                tickStep) *
            tickStep;

        var tickTypeface =
            new Typeface(
                new FontFamily("Segoe UI"),
                FontStyles.Normal,
                FontWeights.Normal,
                FontStretches.Normal);

        for (
            double position = firstTick;
            position <= endPositionMm + 0.001;
            position += tickStep)
        {
            double ratio =
                (position - startPositionMm) /
                range;

            double x =
                left +
                (ratio * rulerWidth);

            bool isMajor =
                IsMajorTick(
                    position,
                    tickStep);

            double tickHeight =
                isMajor
                    ? 18
                    : 10;

            dc.DrawLine(
                isMajor
                    ? majorPen
                    : rulerPen,
                new Point(
                    x,
                    rulerY),
                new Point(
                    x,
                    rulerY - tickHeight));

            if (isMajor)
            {
                string label =
                    $"{position:0}";

                var formatted =
                    new FormattedText(
                        label,
                        System.Globalization.CultureInfo.InvariantCulture,
                        FlowDirection.LeftToRight,
                        tickTypeface,
                        13,
                        rulerBrush,
                        1.0);

                dc.DrawText(
                    formatted,
                    new Point(
                        x - formatted.Width / 2,
                        rulerY + 6));
            }
        }

        var startLabel =
            new FormattedText(
                $"{startPositionMm:0} mm",
                System.Globalization.CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                tickTypeface,
                13,
                majorBrush,
                1.0);

        var endLabel =
            new FormattedText(
                $"{endPositionMm:0} mm",
                System.Globalization.CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                tickTypeface,
                13,
                majorBrush,
                1.0);

        dc.DrawText(
            startLabel,
            new Point(
                left,
                rulerY - 30));

        dc.DrawText(
            endLabel,
            new Point(
                right - endLabel.Width,
                rulerY - 30));
    }

    private static double CalculateTickStep(
        double range)
    {
        if (range <= 100)
        {
            return 10;
        }

        if (range <= 250)
        {
            return 25;
        }

        if (range <= 500)
        {
            return 50;
        }

        return 100;
    }

    private static bool IsMajorTick(
        double position,
        double tickStep)
    {
        if (tickStep <= 0)
        {
            return true;
        }

        double majorStep =
            tickStep * 2;

        double remainder =
            Math.Abs(
                position % majorStep);

        return
            remainder < 0.001 ||
            Math.Abs(
                remainder - majorStep) < 0.001;
    }
}