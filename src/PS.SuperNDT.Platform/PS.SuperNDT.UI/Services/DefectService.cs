using System;
using System.Collections.ObjectModel;
using System.Linq;
using PS.SuperNDT.UI.Database;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class DefectService
{
    private static readonly Lazy<DefectService> _instance =
        new(() => new DefectService());

    public static DefectService Instance =>
        _instance.Value;

    public ObservableCollection<DefectModel> Defects { get; } =
        new();

    private DefectService()
    {
        LoadFromDatabase();
    }

    // ============================================================
    // ADD DEFECT
    // ============================================================

    public DefectModel AddDefect(
        ImageRecordModel image,
        double x,
        double y,
        double width,
        double height)
    {
        ArgumentNullException.ThrowIfNull(image);

        /*
         * IMPORTANT:
         *
         * Do NOT clear previous defects here.
         *
         * One shot can contain multiple defect boxes.
         *
         * Every drawing creates one independent defect record.
         */
        var defect = new DefectModel
        {
            ImageId =
                image.Id,

            JobId =
                image.JobId,

            ShotNumber =
                image.ShotNumber,

            X =
                Math.Max(0, x),

            Y =
                Math.Max(0, y),

            Width =
                Math.Max(0, width),

            Height =
                Math.Max(0, height),

            ShotStartPosition =
                image.ShotStartPosition,

            ShotEndPosition =
                image.ShotEndPosition,

            PipeLength =
                image.PipeLength,

            PipePosition =
                CalculatePipePosition(
                    image,
                    x,
                    width),

            /*
             * No defect measurement/details are entered.
             *
             * Keep these values only for model/database compatibility.
             */
            LengthMm =
                0,

            WidthMm =
                0,

            Status =
                "OPEN",

            Severity =
                string.Empty,

            DefectType =
                string.Empty,

            Description =
                string.Empty,

            CreatedBy =
                Environment.UserName,

            CreatedOn =
                DateTime.Now
        };

        using var db =
            new SuperNDTDbContext();

        db.Initialize();

        db.Defects.Add(defect);

        db.SaveChanges();

        Defects.Add(defect);

        return defect;
    }

    // ============================================================
    // GET BY IMAGE
    // ============================================================

    public ObservableCollection<DefectModel> GetByImage(
        Guid imageId)
    {
        using var db =
            new SuperNDTDbContext();

        db.Initialize();

        var defects =
            db.Defects
              .Where(
                  defect =>
                      defect.ImageId == imageId)
              .OrderBy(
                  defect =>
                      defect.CreatedOn)
              .ThenBy(
                  defect =>
                      defect.Id)
              .ToList();

        /*
         * IMPORTANT:
         *
         * Do NOT remove duplicates.
         *
         * Multiple defect boxes on the same shot are valid.
         */
        return new ObservableCollection<DefectModel>(
            defects);
    }

    // ============================================================
    // GET BY JOB
    // ============================================================

    public ObservableCollection<DefectModel> GetByJob(
        Guid jobId)
    {
        using var db =
            new SuperNDTDbContext();

        db.Initialize();

        var defects =
            db.Defects
              .Where(
                  defect =>
                      defect.JobId == jobId)
              .OrderBy(
                  defect =>
                      defect.ShotNumber)
              .ThenBy(
                  defect =>
                      defect.CreatedOn)
              .ThenBy(
                  defect =>
                      defect.Id)
              .ToList();

        return new ObservableCollection<DefectModel>(
            defects);
    }

    // ============================================================
    // GET BY ID
    // ============================================================

    public DefectModel? GetById(
        Guid defectId)
    {
        using var db =
            new SuperNDTDbContext();

        db.Initialize();

        return db.Defects
                 .FirstOrDefault(
                     defect =>
                         defect.Id == defectId);
    }

    // ============================================================
    // UPDATE DEFECT
    // ============================================================

    /*
     * Kept for compatibility with existing code.
     *
     * The new workflow does not open a detail editor,
     * but other existing code may still call this method.
     */
    public void UpdateDefect(
        DefectModel defect)
    {
        ArgumentNullException.ThrowIfNull(defect);

        using var db =
            new SuperNDTDbContext();

        db.Initialize();

        var existing =
            db.Defects.FirstOrDefault(
                item =>
                    item.Id ==
                    defect.Id);

        if (existing == null)
        {
            return;
        }

        existing.X =
            defect.X;

        existing.Y =
            defect.Y;

        existing.Width =
            defect.Width;

        existing.Height =
            defect.Height;

        existing.PipePosition =
            defect.PipePosition;

        existing.PipeLength =
            defect.PipeLength;

        existing.ShotStartPosition =
            defect.ShotStartPosition;

        existing.ShotEndPosition =
            defect.ShotEndPosition;

        /*
         * Keep these fields blank/neutral.
         * No defect details are collected in the new workflow.
         */
        existing.DefectType =
            string.Empty;

        existing.Severity =
            string.Empty;

        existing.Description =
            string.Empty;

        existing.LengthMm =
            0;

        existing.WidthMm =
            0;

        existing.Status =
            "OPEN";

        existing.UpdatedBy =
            Environment.UserName;

        existing.UpdatedOn =
            DateTime.Now;

        db.SaveChanges();

        var memoryDefect =
            Defects.FirstOrDefault(
                item =>
                    item.Id ==
                    defect.Id);

        if (memoryDefect == null)
        {
            return;
        }

        memoryDefect.X =
            existing.X;

        memoryDefect.Y =
            existing.Y;

        memoryDefect.Width =
            existing.Width;

        memoryDefect.Height =
            existing.Height;

        memoryDefect.PipePosition =
            existing.PipePosition;

        memoryDefect.PipeLength =
            existing.PipeLength;

        memoryDefect.ShotStartPosition =
            existing.ShotStartPosition;

        memoryDefect.ShotEndPosition =
            existing.ShotEndPosition;

        memoryDefect.DefectType =
            existing.DefectType;

        memoryDefect.Severity =
            existing.Severity;

        memoryDefect.Description =
            existing.Description;

        memoryDefect.LengthMm =
            existing.LengthMm;

        memoryDefect.WidthMm =
            existing.WidthMm;

        memoryDefect.Status =
            existing.Status;

        memoryDefect.UpdatedBy =
            existing.UpdatedBy;

        memoryDefect.UpdatedOn =
            existing.UpdatedOn;
    }

    // ============================================================
    // REMOVE ONE DEFECT
    // ============================================================

    public void RemoveDefect(
        Guid defectId)
    {
        using var db =
            new SuperNDTDbContext();

        db.Initialize();

        var defect =
            db.Defects.FirstOrDefault(
                item =>
                    item.Id ==
                    defectId);

        if (defect != null)
        {
            db.Defects.Remove(
                defect);

            db.SaveChanges();
        }

        var memoryDefect =
            Defects.FirstOrDefault(
                item =>
                    item.Id ==
                    defectId);

        if (memoryDefect != null)
        {
            Defects.Remove(
                memoryDefect);
        }
    }

    // ============================================================
    // CLEAR IMAGE
    // ============================================================

    /*
     * This method is intentionally kept.
     *
     * It is used only when the application explicitly wants
     * to remove ALL defect boxes from a shot.
     *
     * AddDefect() NEVER calls this method.
     */
    public void ClearImage(
        Guid imageId)
    {
        using var db =
            new SuperNDTDbContext();

        db.Initialize();

        var defects =
            db.Defects
              .Where(
                  defect =>
                      defect.ImageId ==
                      imageId)
              .ToList();

        if (defects.Count > 0)
        {
            db.Defects.RemoveRange(
                defects);

            db.SaveChanges();
        }

        var memoryDefects =
            Defects
                .Where(
                    defect =>
                        defect.ImageId ==
                        imageId)
                .ToList();

        foreach (var defect in memoryDefects)
        {
            Defects.Remove(
                defect);
        }
    }

    // ============================================================
    // CLEAR JOB
    // ============================================================

    public void ClearJob(
        Guid jobId)
    {
        using var db =
            new SuperNDTDbContext();

        db.Initialize();

        var defects =
            db.Defects
              .Where(
                  defect =>
                      defect.JobId ==
                      jobId)
              .ToList();

        if (defects.Count > 0)
        {
            db.Defects.RemoveRange(
                defects);

            db.SaveChanges();
        }

        var memoryDefects =
            Defects
                .Where(
                    defect =>
                        defect.JobId ==
                        jobId)
                .ToList();

        foreach (var defect in memoryDefects)
        {
            Defects.Remove(
                defect);
        }
    }

    // ============================================================
    // CLEAR ALL
    // ============================================================

    public void ClearAll()
    {
        using var db =
            new SuperNDTDbContext();

        db.Initialize();

        var defects =
            db.Defects.ToList();

        if (defects.Count > 0)
        {
            db.Defects.RemoveRange(
                defects);

            db.SaveChanges();
        }

        Defects.Clear();
    }

    // ============================================================
    // LOAD DATABASE
    // ============================================================

    private void LoadFromDatabase()
    {
        try
        {
            using var db =
                new SuperNDTDbContext();

            db.Initialize();

            var defects =
                db.Defects
                  .OrderBy(
                      defect =>
                          defect.CreatedOn)
                  .ThenBy(
                      defect =>
                          defect.Id)
                  .ToList();

            foreach (var defect in defects)
            {
                Defects.Add(
                    defect);
            }
        }
        catch
        {
            /*
             * Database initialization can happen
             * later during application startup.
             */
        }
    }

    // ============================================================
    // PIPE POSITION
    // ============================================================

    private static double CalculatePipePosition(
        ImageRecordModel image,
        double x,
        double width)
    {
        if (image.ImageWidth <= 0)
        {
            return image.ShotStartPosition;
        }

        double imageCenter =
            x +
            (width / 2.0);

        double ratio =
            imageCenter /
            image.ImageWidth;

        ratio =
            Math.Clamp(
                ratio,
                0.0,
                1.0);

        double shotLength =
            image.ShotEndPosition -
            image.ShotStartPosition;

        if (shotLength <= 0)
        {
            shotLength =
                image.ShotSize;
        }

        if (shotLength <= 0)
        {
            return image.ShotStartPosition;
        }

        return image.ShotStartPosition +
               (shotLength * ratio);
    }
}