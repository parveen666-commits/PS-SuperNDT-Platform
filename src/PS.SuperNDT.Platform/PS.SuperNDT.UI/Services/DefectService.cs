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

    public DefectModel AddDefect(
        ImageRecordModel image,
        double x,
        double y,
        double width,
        double height)
    {
        ArgumentNullException.ThrowIfNull(image);

        /*
         * Review workflow uses one saved defect mark per shot/image.
         *
         * Remove any previous record for this image before creating
         * the new one. This also prevents an old defect from coming
         * back when the user changes shot, accepts another shot and
         * later returns to this image.
         */
        ClearImage(image.Id);

        var defect = new DefectModel
        {
            ImageId =
                image.Id,

            JobId =
                image.JobId,

            ShotNumber =
                image.ShotNumber,

            X =
                x,

            Y =
                y,

            Width =
                width,

            Height =
                height,

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

            LengthMm =
                0,

            WidthMm =
                0,

            Status =
                "OPEN",

            Severity =
                "UNCLASSIFIED",

            DefectType =
                "UNCLASSIFIED",

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
              .OrderByDescending(
                  defect =>
                      defect.CreatedOn)
              .ThenByDescending(
                  defect =>
                      defect.Id)
              .ToList();

        /*
         * Self-heal old duplicate records created by the previous
         * defect workflow.
         *
         * Only the latest saved defect for an image is valid in the
         * current Review workflow.
         */
        if (defects.Count > 1)
        {
            var latest =
                defects[0];

            var obsolete =
                defects
                    .Skip(1)
                    .ToList();

            db.Defects.RemoveRange(
                obsolete);

            db.SaveChanges();

            foreach (var oldDefect in obsolete)
            {
                var memoryDefect =
                    Defects.FirstOrDefault(
                        item =>
                            item.Id ==
                            oldDefect.Id);

                if (memoryDefect != null)
                {
                    Defects.Remove(
                        memoryDefect);
                }
            }

            defects =
                new[] { latest }
                    .ToList();
        }

        return new ObservableCollection<DefectModel>(
            defects);
    }

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
              .ToList();

        return new ObservableCollection<DefectModel>(
            defects);
    }

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

    public void UpdateDefect(
        DefectModel defect)
    {
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

        existing.DefectType =
            defect.DefectType;

        existing.Description =
            defect.Description;

        existing.X =
            defect.X;

        existing.Y =
            defect.Y;

        existing.Width =
            defect.Width;

        existing.Height =
            defect.Height;

        existing.LengthMm =
            defect.LengthMm;

        existing.WidthMm =
            defect.WidthMm;

        existing.PipePosition =
            defect.PipePosition;

        existing.PipeLength =
            defect.PipeLength;

        existing.ShotStartPosition =
            defect.ShotStartPosition;

        existing.ShotEndPosition =
            defect.ShotEndPosition;

        existing.Severity =
            defect.Severity;

        existing.Status =
            defect.Status;

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

        memoryDefect.DefectType =
            existing.DefectType;

        memoryDefect.Description =
            existing.Description;

        memoryDefect.X =
            existing.X;

        memoryDefect.Y =
            existing.Y;

        memoryDefect.Width =
            existing.Width;

        memoryDefect.Height =
            existing.Height;

        memoryDefect.LengthMm =
            existing.LengthMm;

        memoryDefect.WidthMm =
            existing.WidthMm;

        memoryDefect.PipePosition =
            existing.PipePosition;

        memoryDefect.PipeLength =
            existing.PipeLength;

        memoryDefect.ShotStartPosition =
            existing.ShotStartPosition;

        memoryDefect.ShotEndPosition =
            existing.ShotEndPosition;

        memoryDefect.Severity =
            existing.Severity;

        memoryDefect.Status =
            existing.Status;

        memoryDefect.UpdatedBy =
            existing.UpdatedBy;

        memoryDefect.UpdatedOn =
            existing.UpdatedOn;
    }

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
                  .ToList();

            foreach (var defect in defects)
            {
                Defects.Add(
                    defect);
            }
        }
        catch
        {
            // Database initialization can occur
            // later during application startup.
        }
    }

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