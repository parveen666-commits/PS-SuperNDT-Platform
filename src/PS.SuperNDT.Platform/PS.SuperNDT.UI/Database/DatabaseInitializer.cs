using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace PS.SuperNDT.UI.Database;

public static class DatabaseInitializer
{
    private static bool _initialized;

    public static void Initialize()
    {
        if (_initialized)
            return;

        try
        {
            using var db = new SuperNDTDbContext();

            db.Initialize();

            EnsureImageColumns();

            _initialized = true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "Failed to initialize PS SuperNDT database.",
                ex);
        }
    }

    private static void EnsureImageColumns()
    {
        using var connection =
            new SqliteConnection("Data Source=PS_SuperNDT.db");

        connection.Open();

        var existingColumns =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        using (var command = connection.CreateCommand())
        {
            command.CommandText = "PRAGMA table_info(Images);";

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                existingColumns.Add(reader.GetString(1));
            }
        }

        var columns = new Dictionary<string, string>(
            StringComparer.OrdinalIgnoreCase)
        {
            ["JobId"] = "TEXT NOT NULL DEFAULT ''",
            ["JobNumber"] = "TEXT NOT NULL DEFAULT ''",
            ["PipeId"] = "TEXT NOT NULL DEFAULT ''",
            ["Operator"] = "TEXT NOT NULL DEFAULT ''",
            ["Remarks"] = "TEXT NOT NULL DEFAULT ''",
            ["FrameNumber"] = "INTEGER NOT NULL DEFAULT 0",
            ["FileName"] = "TEXT NOT NULL DEFAULT ''",
            ["FilePath"] = "TEXT NOT NULL DEFAULT ''",
            ["DetectorName"] = "TEXT NOT NULL DEFAULT ''",
            ["KV"] = "REAL NOT NULL DEFAULT 0",
            ["MA"] = "REAL NOT NULL DEFAULT 0",
            ["ExposureTime"] = "REAL NOT NULL DEFAULT 0",
            ["ImageWidth"] = "INTEGER NOT NULL DEFAULT 0",
            ["ImageHeight"] = "INTEGER NOT NULL DEFAULT 0",
            ["BitDepth"] = "INTEGER NOT NULL DEFAULT 0",
            ["CapturedOn"] = "TEXT NOT NULL DEFAULT ''",
            ["ShotNumber"] = "INTEGER NOT NULL DEFAULT 0",
            ["TotalShots"] = "INTEGER NOT NULL DEFAULT 0",
            ["PipeLength"] = "REAL NOT NULL DEFAULT 0",
            ["ShotSize"] = "REAL NOT NULL DEFAULT 0",
            ["Overlap"] = "REAL NOT NULL DEFAULT 0",
            ["ShotStartPosition"] = "REAL NOT NULL DEFAULT 0",
            ["ShotEndPosition"] = "REAL NOT NULL DEFAULT 0",
            ["ReviewStatus"] = "TEXT NOT NULL DEFAULT 'PENDING'",
            ["ReviewedBy"] = "TEXT NOT NULL DEFAULT ''",
            ["ReviewedOn"] = "TEXT NULL"
        };

        foreach (var column in columns)
        {
            if (existingColumns.Contains(column.Key))
                continue;

            using var alterCommand =
                connection.CreateCommand();

            alterCommand.CommandText =
                $"ALTER TABLE Images ADD COLUMN \"{column.Key}\" {column.Value};";

            alterCommand.ExecuteNonQuery();
        }
    }
}