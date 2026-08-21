using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PS.SuperNDT.UI.Database;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class RtrReviewFilterService
{
    public List<ImageRecordModel> GetFiltered(
        RtrReviewFilterModel filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        using var db = new SuperNDTDbContext();

        IQueryable<ImageRecordModel> query =
            db.Set<ImageRecordModel>()
              .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.ImageName))
        {
            string value = filter.ImageName.Trim();

            query = query.Where(x =>
                x.FileName.Contains(value));
        }

        if (!string.IsNullOrWhiteSpace(filter.WeldNumber))
        {
            string value = filter.WeldNumber.Trim();

            query = query.Where(x =>
                x.WeldNumber.Contains(value));
        }

        if (!string.IsNullOrWhiteSpace(filter.JointNumber))
        {
            string value = filter.JointNumber.Trim();

            query = query.Where(x =>
                x.JointNumber.Contains(value));
        }

        if (!string.IsNullOrWhiteSpace(filter.WeldType))
        {
            string value = filter.WeldType.Trim();

            query = query.Where(x =>
                x.WeldType == value);
        }

        if (!string.IsNullOrWhiteSpace(filter.WeldingProcess))
        {
            string value = filter.WeldingProcess.Trim();

            query = query.Where(x =>
                x.WeldingProcess == value);
        }

        // IQI

        if (!string.IsNullOrWhiteSpace(filter.IqiType))
        {
            string value = filter.IqiType.Trim();

            query = query.Where(x =>
                x.IQIType == value);
        }

        if (!string.IsNullOrWhiteSpace(filter.IqiSensitivity))
        {
            string value = filter.IqiSensitivity.Trim();

            query = query.Where(x =>
                x.IQISensitivity.Contains(value));
        }

        if (filter.IqiMinimum.HasValue)
        {
            double minimum = filter.IqiMinimum.Value;

            query = query.Where(x =>
                x.IQI >= minimum);
        }

        if (filter.IqiMaximum.HasValue)
        {
            double maximum = filter.IqiMaximum.Value;

            query = query.Where(x =>
                x.IQI <= maximum);
        }

        // Filter / Grain

        if (!string.IsNullOrWhiteSpace(filter.Filter))
        {
            string value = filter.Filter.Trim();

            query = query.Where(x =>
                x.Filter == value);
        }

        if (!string.IsNullOrWhiteSpace(filter.Grain))
        {
            string value = filter.Grain.Trim();

            query = query.Where(x =>
                x.Grain.Contains(value));
        }

        // Defect

        if (!string.IsNullOrWhiteSpace(filter.DefectType))
        {
            string value = filter.DefectType.Trim();

            query = query.Where(x =>
                db.Set<DefectModel>()
                    .Any(d =>
                        d.ImageId == x.Id &&
                        d.DefectType == value));
        }

        if (!string.IsNullOrWhiteSpace(filter.AcceptanceCode))
        {
            string value = filter.AcceptanceCode.Trim();

            query = query.Where(x =>
                db.Set<DefectModel>()
                    .Any(d =>
                        d.ImageId == x.Id &&
                        d.Status == value));
        }

        if (!string.IsNullOrWhiteSpace(filter.Result))
        {
            string value = filter.Result.Trim();

            query = query.Where(x =>
                x.ReviewStatus == value);
        }

        // SNR

        if (filter.SnrMinimum.HasValue)
        {
            double minimum = filter.SnrMinimum.Value;

            query = query.Where(x =>
                x.SNR >= minimum);
        }

        if (filter.SnrMaximum.HasValue)
        {
            double maximum = filter.SnrMaximum.Value;

            query = query.Where(x =>
                x.SNR <= maximum);
        }

        // Density

        if (filter.DensityMinimum.HasValue)
        {
            double minimum = filter.DensityMinimum.Value;

            query = query.Where(x =>
                x.Density >= minimum);
        }

        if (filter.DensityMaximum.HasValue)
        {
            double maximum = filter.DensityMaximum.Value;

            query = query.Where(x =>
                x.Density <= maximum);
        }

        // Contrast

        if (filter.ContrastMinimum.HasValue)
        {
            double minimum = filter.ContrastMinimum.Value;

            query = query.Where(x =>
                x.Contrast >= minimum);
        }

        if (filter.ContrastMaximum.HasValue)
        {
            double maximum = filter.ContrastMaximum.Value;

            query = query.Where(x =>
                x.Contrast <= maximum);
        }

        // Basic Spatial Resolution

        if (filter.BasicSpatialResolutionMaximum.HasValue)
        {
            double maximum =
                filter.BasicSpatialResolutionMaximum.Value;

            query = query.Where(x =>
                x.BasicSpatialResolution <= maximum);
        }

        // kV

        if (filter.KvMinimum.HasValue)
        {
            double minimum = filter.KvMinimum.Value;

            query = query.Where(x =>
                x.KV >= minimum);
        }

        if (filter.KvMaximum.HasValue)
        {
            double maximum = filter.KvMaximum.Value;

            query = query.Where(x =>
                x.KV <= maximum);
        }

        // mA

        if (filter.MaMinimum.HasValue)
        {
            double minimum = filter.MaMinimum.Value;

            query = query.Where(x =>
                x.MA >= minimum);
        }

        if (filter.MaMaximum.HasValue)
        {
            double maximum = filter.MaMaximum.Value;

            query = query.Where(x =>
                x.MA <= maximum);
        }

        // Exposure Time

        if (filter.ExposureTimeMinimum.HasValue)
        {
            double minimum =
                filter.ExposureTimeMinimum.Value;

            query = query.Where(x =>
                x.ExposureTime >= minimum);
        }

        if (filter.ExposureTimeMaximum.HasValue)
        {
            double maximum =
                filter.ExposureTimeMaximum.Value;

            query = query.Where(x =>
                x.ExposureTime <= maximum);
        }

        // SFD

        if (filter.SfdMinimum.HasValue)
        {
            double minimum = filter.SfdMinimum.Value;

            query = query.Where(x =>
                x.SFD >= minimum);
        }

        if (filter.SfdMaximum.HasValue)
        {
            double maximum = filter.SfdMaximum.Value;

            query = query.Where(x =>
                x.SFD <= maximum);
        }

        // ODD

        if (filter.OddMinimum.HasValue)
        {
            double minimum = filter.OddMinimum.Value;

            query = query.Where(x =>
                x.ODD >= minimum);
        }

        if (filter.OddMaximum.HasValue)
        {
            double maximum = filter.OddMaximum.Value;

            query = query.Where(x =>
                x.ODD <= maximum);
        }

        // Geometric Unsharpness

        if (filter.UnsharpnessMaximum.HasValue)
        {
            double maximum =
                filter.UnsharpnessMaximum.Value;

            query = query.Where(x =>
                x.GeometricUnsharpness <= maximum);
        }

        // Material Thickness

        if (filter.MaterialThicknessMinimum.HasValue)
        {
            double minimum =
                filter.MaterialThicknessMinimum.Value;

            query = query.Where(x =>
                x.MaterialThickness >= minimum);
        }

        if (filter.MaterialThicknessMaximum.HasValue)
        {
            double maximum =
                filter.MaterialThicknessMaximum.Value;

            query = query.Where(x =>
                x.MaterialThickness <= maximum);
        }

        // Date

        if (filter.FromDate.HasValue)
        {
            DateTime from =
                filter.FromDate.Value.Date;

            query = query.Where(x =>
                x.CapturedOn >= from);
        }

        if (filter.ToDate.HasValue)
        {
            DateTime to =
                filter.ToDate.Value.Date
                    .AddDays(1)
                    .AddTicks(-1);

            query = query.Where(x =>
                x.CapturedOn <= to);
        }

        // Review Status

        if (filter.ReviewedOnly)
        {
            query = query.Where(x =>
                x.ReviewStatus != "PENDING");
        }

        if (filter.AcceptedOnly)
        {
            query = query.Where(x =>
                x.ReviewStatus == "ACCEPTED");
        }

        if (filter.RejectedOnly)
        {
            query = query.Where(x =>
                x.ReviewStatus == "REJECTED");
        }

        return query
            .OrderByDescending(x => x.CapturedOn)
            .ThenBy(x => x.ShotNumber)
            .ToList();
    }

    public List<string> GetWeldTypes()
    {
        using var db = new SuperNDTDbContext();

        return db.Set<ImageRecordModel>()
            .AsNoTracking()
            .Where(x => x.WeldType != "")
            .Select(x => x.WeldType)
            .Distinct()
            .OrderBy(x => x)
            .ToList();
    }

    public List<string> GetWeldingProcesses()
    {
        using var db = new SuperNDTDbContext();

        return db.Set<ImageRecordModel>()
            .AsNoTracking()
            .Where(x => x.WeldingProcess != "")
            .Select(x => x.WeldingProcess)
            .Distinct()
            .OrderBy(x => x)
            .ToList();
    }

    public List<string> GetIqiTypes()
    {
        using var db = new SuperNDTDbContext();

        return db.Set<ImageRecordModel>()
            .AsNoTracking()
            .Where(x => x.IQIType != "")
            .Select(x => x.IQIType)
            .Distinct()
            .OrderBy(x => x)
            .ToList();
    }

    public List<string> GetFilters()
    {
        using var db = new SuperNDTDbContext();

        return db.Set<ImageRecordModel>()
            .AsNoTracking()
            .Where(x => x.Filter != "")
            .Select(x => x.Filter)
            .Distinct()
            .OrderBy(x => x)
            .ToList();
    }

    public List<string> GetGrains()
    {
        using var db = new SuperNDTDbContext();

        return db.Set<ImageRecordModel>()
            .AsNoTracking()
            .Where(x => x.Grain != "")
            .Select(x => x.Grain)
            .Distinct()
            .OrderBy(x => x)
            .ToList();
    }

    public List<string> GetDefectTypes()
    {
        using var db = new SuperNDTDbContext();

        return db.Set<DefectModel>()
            .AsNoTracking()
            .Where(x => x.DefectType != "")
            .Select(x => x.DefectType)
            .Distinct()
            .OrderBy(x => x)
            .ToList();
    }

    public List<string> GetDefectStatuses()
    {
        using var db = new SuperNDTDbContext();

        return db.Set<DefectModel>()
            .AsNoTracking()
            .Where(x => x.Status != "")
            .Select(x => x.Status)
            .Distinct()
            .OrderBy(x => x)
            .ToList();
    }
}