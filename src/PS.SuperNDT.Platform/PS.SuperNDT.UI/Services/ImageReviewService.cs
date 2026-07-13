using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PS.SuperNDT.UI.Database;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ImageReviewService
{
    public void Save(ImageReviewModel review)
    {
        ArgumentNullException.ThrowIfNull(review);

        using var db = new SuperNDTDbContext();

        var existing = db.Set<ImageReviewModel>()
            .FirstOrDefault(x => x.Id == review.Id);

        if (existing == null)
        {
            db.Set<ImageReviewModel>().Add(review);
        }
        else
        {
            db.Entry(existing)
                .CurrentValues
                .SetValues(review);
        }

        db.SaveChanges();
    }

    public ImageReviewModel? Get(Guid id)
    {
        using var db = new SuperNDTDbContext();

        return db.Set<ImageReviewModel>()
            .AsNoTracking()
            .FirstOrDefault(x => x.Id == id);
    }

    public List<ImageReviewModel> GetByExposure(Guid exposureId)
    {
        using var db = new SuperNDTDbContext();

        return db.Set<ImageReviewModel>()
            .AsNoTracking()
            .Where(x => x.ExposureId == exposureId)
            .OrderByDescending(x => x.ReviewDate)
            .ToList();
    }

    public List<ImageReviewModel> GetAll()
    {
        using var db = new SuperNDTDbContext();

        return db.Set<ImageReviewModel>()
            .AsNoTracking()
            .OrderByDescending(x => x.ReviewDate)
            .ToList();
    }

    public void MarkAccepted(Guid reviewId)
    {
        using var db = new SuperNDTDbContext();

        var review = db.Set<ImageReviewModel>()
            .FirstOrDefault(x => x.Id == reviewId);

        if (review == null)
            return;

        review.IsAccepted = true;
        review.IsReviewed = true;
        review.Result = "Accepted";
        review.ReviewDate = DateTime.Now;

        db.SaveChanges();
    }

    public void MarkRejected(Guid reviewId)
    {
        using var db = new SuperNDTDbContext();

        var review = db.Set<ImageReviewModel>()
            .FirstOrDefault(x => x.Id == reviewId);

        if (review == null)
            return;

        review.IsAccepted = false;
        review.IsReviewed = true;
        review.Result = "Rejected";
        review.ReviewDate = DateTime.Now;

        db.SaveChanges();
    }

    public void Delete(Guid id)
    {
        using var db = new SuperNDTDbContext();

        var review = db.Set<ImageReviewModel>()
            .FirstOrDefault(x => x.Id == id);

        if (review == null)
            return;

        db.Set<ImageReviewModel>().Remove(review);

        db.SaveChanges();
    }
}