using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PS.SuperNDT.UI.Database;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ImageService
{
    public void Save(ImageRecordModel image)
    {
        using var db = new SuperNDTDbContext();

        var existing =
            db.Images.FirstOrDefault(x => x.Id == image.Id);

        if (existing == null)
        {
            db.Images.Add(image);
        }
        else
        {
            db.Entry(existing)
              .CurrentValues
              .SetValues(image);
        }

        db.SaveChanges();
    }

    public List<ImageRecordModel> GetAll()
    {
        using var db = new SuperNDTDbContext();

        return db.Images
                 .AsNoTracking()
                 .OrderByDescending(x => x.CapturedOn)
                 .ToList();
    }

    public List<ImageRecordModel> GetByJob(Guid jobId)
    {
        using var db = new SuperNDTDbContext();

        return db.Images
                 .AsNoTracking()
                 .Where(x => x.JobId == jobId)
                 .OrderByDescending(x => x.CapturedOn)
                 .ToList();
    }

    public int GetImageCount(Guid jobId)
    {
        using var db = new SuperNDTDbContext();

        return db.Images.Count(x => x.JobId == jobId);
    }

    public int GetTotalImageCount()
    {
        using var db = new SuperNDTDbContext();

        return db.Images.Count();
    }

    public ImageRecordModel? Get(Guid id)
    {
        using var db = new SuperNDTDbContext();

        return db.Images
                 .AsNoTracking()
                 .FirstOrDefault(x => x.Id == id);
    }

    public void Delete(Guid id)
    {
        using var db = new SuperNDTDbContext();

        var image =
            db.Images.FirstOrDefault(x => x.Id == id);

        if (image == null)
            return;

        db.Images.Remove(image);

        db.SaveChanges();
    }
}