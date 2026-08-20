using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PS.SuperNDT.UI.Database;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ImageService
{
    public void Save(ImageRecordModel image)
    {
        EnsureImageSchema();

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
        EnsureImageSchema();

        using var db = new SuperNDTDbContext();

        return db.Images
                 .AsNoTracking()
                 .OrderByDescending(x => x.CapturedOn)
                 .ToList();
    }

    public List<ImageRecordModel> GetByJob(Guid jobId)
    {
        EnsureImageSchema();

        using var db = new SuperNDTDbContext();

        return db.Images
                 .AsNoTracking()
                 .Where(x => x.JobId == jobId)
                 .OrderByDescending(x => x.CapturedOn)
                 .ToList();
    }

    public int GetImageCount(Guid jobId)
    {
        EnsureImageSchema();

        using var db = new SuperNDTDbContext();

        return db.Images.Count(x => x.JobId == jobId);
    }

    public int GetTotalImageCount()
    {
        EnsureImageSchema();

        using var db = new SuperNDTDbContext();

        return db.Images.Count();
    }

    public ImageRecordModel? Get(Guid id)
    {
        EnsureImageSchema();

        using var db = new SuperNDTDbContext();

        return db.Images
                 .AsNoTracking()
                 .FirstOrDefault(x => x.Id == id);
    }

    public void Delete(Guid id)
    {
        EnsureImageSchema();

        using var db = new SuperNDTDbContext();

        var image =
            db.Images.FirstOrDefault(x => x.Id == id);

        if (image == null)
        {
            return;
        }

        db.Images.Remove(image);

        db.SaveChanges();
    }

    private static void EnsureImageSchema()
    {
        using var connection =
            new SqliteConnection(
                "Data Source=PS_SuperNDT.db");

        connection.Open();

        AddColumnIfMissing(
            connection,
            "PipeId",
            "TEXT NOT NULL DEFAULT ''");

        AddColumnIfMissing(
            connection,
            "ShotNumber",
            "INTEGER NOT NULL DEFAULT 0");

        AddColumnIfMissing(
            connection,
            "TotalShots",
            "INTEGER NOT NULL DEFAULT 0");

        AddColumnIfMissing(
            connection,
            "PipeLength",
            "REAL NOT NULL DEFAULT 0");

        AddColumnIfMissing(
            connection,
            "ShotSize",
            "REAL NOT NULL DEFAULT 0");

        AddColumnIfMissing(
            connection,
            "Overlap",
            "REAL NOT NULL DEFAULT 0");

        AddColumnIfMissing(
            connection,
            "ShotStartPosition",
            "REAL NOT NULL DEFAULT 0");

        AddColumnIfMissing(
            connection,
            "ShotEndPosition",
            "REAL NOT NULL DEFAULT 0");

        AddColumnIfMissing(
            connection,
            "ReviewStatus",
            "TEXT NOT NULL DEFAULT 'PENDING'");

        AddColumnIfMissing(
            connection,
            "ReviewedBy",
            "TEXT NOT NULL DEFAULT ''");

        AddColumnIfMissing(
            connection,
            "ReviewedOn",
            "TEXT NULL");
    }

    private static void AddColumnIfMissing(
        SqliteConnection connection,
        string columnName,
        string columnDefinition)
    {
        using var checkCommand =
            connection.CreateCommand();

        checkCommand.CommandText =
            "SELECT COUNT(*) " +
            "FROM pragma_table_info('Images') " +
            "WHERE name = $columnName;";

        checkCommand.Parameters.AddWithValue(
            "$columnName",
            columnName);

        var exists =
            Convert.ToInt32(
                checkCommand.ExecuteScalar()) > 0;

        if (exists)
        {
            return;
        }

        using var alterCommand =
            connection.CreateCommand();

        alterCommand.CommandText =
            $"ALTER TABLE Images " +
            $"ADD COLUMN \"{columnName}\" {columnDefinition};";

        alterCommand.ExecuteNonQuery();
    }
}