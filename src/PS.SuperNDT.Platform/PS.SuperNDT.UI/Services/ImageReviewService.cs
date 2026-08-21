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

    public void SaveReviewDecision(
        ImageReviewModel review,
        string result,
        string reviewer)
    {
        ArgumentNullException.ThrowIfNull(review);

        using var db = new SuperNDTDbContext();

        var existing = db.Set<ImageReviewModel>()
            .FirstOrDefault(x => x.Id == review.Id);

        if (existing == null)
        {
            existing = review;
            db.Set<ImageReviewModel>().Add(existing);
        }
        else
        {
            existing.ImageName = review.ImageName;
            existing.FilePath = review.FilePath;
            existing.Reviewer = reviewer;
            existing.ReviewDate = DateTime.Now;
            existing.Result = result;
            existing.DefectType = review.DefectType;
            existing.DefectLocation = review.DefectLocation;
            existing.DefectLength = review.DefectLength;
            existing.DefectWidth = review.DefectWidth;
            existing.AcceptanceCode = review.AcceptanceCode;
            existing.Remarks = review.Remarks;
            existing.IsAccepted = result == "ACCEPTED";
            existing.IsReviewed = result != "PENDING";
            existing.ZoomLevel = review.ZoomLevel;
            existing.Brightness = review.Brightness;
            existing.Contrast = review.Contrast;
        }

        existing.Reviewer = reviewer;
        existing.ReviewDate = DateTime.Now;
        existing.Result = result;
        existing.IsAccepted = result == "ACCEPTED";
        existing.IsReviewed = result != "PENDING";

        db.SaveChanges();

        SyncImageRecord(
            db,
            existing,
            result,
            reviewer);
    }

    private static void SyncImageRecord(
        SuperNDTDbContext db,
        ImageReviewModel review,
        string result,
        string reviewer)
    {
        if (string.IsNullOrWhiteSpace(review.ImageName) &&
            string.IsNullOrWhiteSpace(review.FilePath))
        {
            return;
        }

        var image = db.Set<ImageRecordModel>()
            .FirstOrDefault(x =>
                (!string.IsNullOrWhiteSpace(review.ImageName) &&
                 x.FileName == review.ImageName)
                ||
                (!string.IsNullOrWhiteSpace(review.FilePath) &&
                 x.FilePath == review.FilePath));

        if (image == null)
            return;

        image.ReviewStatus = result;
        image.ReviewedBy = reviewer;
        image.ReviewedOn = DateTime.Now;

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