using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Database;

public sealed class SuperNDTDbContext : DbContext
{
    public DbSet<JobModel> Jobs => Set<JobModel>();

    public DbSet<ImageRecordModel> Images => Set<ImageRecordModel>();

    public DbSet<CustomerModel> Customers => Set<CustomerModel>();

    public DbSet<DefectModel> Defects => Set<DefectModel>();

    protected override void OnConfiguring(
        DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite(
                "Data Source=PS_SuperNDT.db");
        }
    }

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<JobModel>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.JobNumber)
                .HasMaxLength(100);

            entity.Property(e => e.Customer)
                .HasMaxLength(200);

            entity.Property(e => e.Project)
                .HasMaxLength(200);

            entity.Property(e => e.Component)
                .HasMaxLength(200);

            entity.Property(e => e.WeldNumber)
                .HasMaxLength(100);

            entity.Property(e => e.Operator)
                .HasMaxLength(100);

            entity.Property(e => e.Procedure)
                .HasMaxLength(200);

            entity.Property(e => e.Material)
                .HasMaxLength(100);

            entity.Property(e => e.Remark)
                .HasMaxLength(1000);
        });

        modelBuilder.Entity<ImageRecordModel>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.JobNumber)
                .HasMaxLength(100);

            entity.Property(e => e.PipeId)
                .HasMaxLength(100);

            entity.Property(e => e.Operator)
                .HasMaxLength(100);

            entity.Property(e => e.FileName)
                .HasMaxLength(300);

            entity.Property(e => e.FilePath)
                .HasMaxLength(500);

            entity.Property(e => e.DetectorName)
                .HasMaxLength(100);

            entity.Property(e => e.IQIType)
                .HasMaxLength(100);

            entity.Property(e => e.IQISensitivity)
                .HasMaxLength(100);

            entity.Property(e => e.Filter)
                .HasMaxLength(100);

            entity.Property(e => e.Grain)
                .HasMaxLength(100);

            entity.Property(e => e.WeldNumber)
                .HasMaxLength(100);

            entity.Property(e => e.JointNumber)
                .HasMaxLength(100);

            entity.Property(e => e.WeldType)
                .HasMaxLength(100);

            entity.Property(e => e.WeldingProcess)
                .HasMaxLength(100);

            entity.Property(e => e.WeldOrientation)
                .HasMaxLength(100);

            entity.Property(e => e.ReviewStatus)
                .HasMaxLength(50);

            entity.Property(e => e.ReviewedBy)
                .HasMaxLength(100);

            entity.Property(e => e.Remarks)
                .HasMaxLength(1000);
        });

        modelBuilder.Entity<DefectModel>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.DefectType)
                .HasMaxLength(100);

            entity.Property(e => e.Description)
                .HasMaxLength(2000);

            entity.Property(e => e.Severity)
                .HasMaxLength(100);

            entity.Property(e => e.Status)
                .HasMaxLength(100);

            entity.Property(e => e.ThicknessStatus)
                .HasMaxLength(50);

            entity.Property(e => e.ThicknessRemark)
                .HasMaxLength(1000);

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100);

            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100);
        });

        base.OnModelCreating(modelBuilder);
    }

    public void Initialize()
    {
        Database.EnsureCreated();

        EnsureImageTableSchema();

        EnsureDefectTableSchema();
    }

    private void EnsureImageTableSchema()
    {
        var requiredColumns =
            new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase)
            {
                ["ShotNumber"] =
                    "INTEGER NOT NULL DEFAULT 0",

                ["TotalShots"] =
                    "INTEGER NOT NULL DEFAULT 0",

                ["PipeLength"] =
                    "REAL NOT NULL DEFAULT 0",

                ["ShotSize"] =
                    "REAL NOT NULL DEFAULT 0",

                ["Overlap"] =
                    "REAL NOT NULL DEFAULT 0",

                ["ShotStartPosition"] =
                    "REAL NOT NULL DEFAULT 0",

                ["ShotEndPosition"] =
                    "REAL NOT NULL DEFAULT 0",

                ["ReviewStatus"] =
                    "TEXT NOT NULL DEFAULT 'PENDING'",

                ["ReviewedBy"] =
                    "TEXT NOT NULL DEFAULT ''",

                ["ReviewedOn"] =
                    "TEXT NULL",

                ["SNR"] =
                    "REAL NOT NULL DEFAULT 0",

                ["IQI"] =
                    "REAL NOT NULL DEFAULT 0",

                ["IQIType"] =
                    "TEXT NOT NULL DEFAULT ''",

                ["IQISensitivity"] =
                    "TEXT NOT NULL DEFAULT ''",

                ["Filter"] =
                    "TEXT NOT NULL DEFAULT ''",

                ["Grain"] =
                    "TEXT NOT NULL DEFAULT ''",

                ["SFD"] =
                    "REAL NOT NULL DEFAULT 0",

                ["ODD"] =
                    "REAL NOT NULL DEFAULT 0",

                ["GeometricUnsharpness"] =
                    "REAL NOT NULL DEFAULT 0",

                ["Density"] =
                    "REAL NOT NULL DEFAULT 0",

                ["Contrast"] =
                    "REAL NOT NULL DEFAULT 0",

                ["BasicSpatialResolution"] =
                    "REAL NOT NULL DEFAULT 0",

                ["WeldNumber"] =
                    "TEXT NOT NULL DEFAULT ''",

                ["JointNumber"] =
                    "TEXT NOT NULL DEFAULT ''",

                ["WeldType"] =
                    "TEXT NOT NULL DEFAULT ''",

                ["WeldingProcess"] =
                    "TEXT NOT NULL DEFAULT ''",

                ["WeldOrientation"] =
                    "TEXT NOT NULL DEFAULT ''",

                ["MaterialThickness"] =
                    "REAL NOT NULL DEFAULT 0"
            };

        var connection =
            Database.GetDbConnection();

        bool shouldClose =
            connection.State !=
            System.Data.ConnectionState.Open;

        if (shouldClose)
        {
            connection.Open();
        }

        try
        {
            var existingColumns =
                new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase);

            using var pragmaCommand =
                connection.CreateCommand();

            pragmaCommand.CommandText =
                "PRAGMA table_info(Images);";

            using var reader =
                pragmaCommand.ExecuteReader();

            while (reader.Read())
            {
                existingColumns.Add(
                    reader.GetString(1));
            }

            foreach (var column in requiredColumns)
            {
                if (existingColumns.Contains(
                    column.Key))
                {
                    continue;
                }

                using var alterCommand =
                    connection.CreateCommand();

                alterCommand.CommandText =
                    $"ALTER TABLE Images " +
                    $"ADD COLUMN [{column.Key}] " +
                    $"{column.Value};";

                alterCommand.ExecuteNonQuery();
            }
        }
        finally
        {
            if (shouldClose)
            {
                connection.Close();
            }
        }
    }

    private void EnsureDefectTableSchema()
    {
        var connection =
            Database.GetDbConnection();

        bool shouldClose =
            connection.State !=
            System.Data.ConnectionState.Open;

        if (shouldClose)
        {
            connection.Open();
        }

        try
        {
            using var createCommand =
                connection.CreateCommand();

            createCommand.CommandText =
                """
                CREATE TABLE IF NOT EXISTS Defects
                (
                    Id TEXT NOT NULL PRIMARY KEY,
                    ImageId TEXT NOT NULL,
                    JobId TEXT NOT NULL,
                    ShotNumber INTEGER NOT NULL DEFAULT 0,
                    DefectType TEXT NOT NULL DEFAULT '',
                    Description TEXT NOT NULL DEFAULT '',
                    X REAL NOT NULL DEFAULT 0,
                    Y REAL NOT NULL DEFAULT 0,
                    Width REAL NOT NULL DEFAULT 0,
                    Height REAL NOT NULL DEFAULT 0,
                    LengthMm REAL NOT NULL DEFAULT 0,
                    WidthMm REAL NOT NULL DEFAULT 0,
                    PipePosition REAL NOT NULL DEFAULT 0,
                    PipeLength REAL NOT NULL DEFAULT 0,
                    ShotStartPosition REAL NOT NULL DEFAULT 0,
                    ShotEndPosition REAL NOT NULL DEFAULT 0,
                    Severity TEXT NOT NULL DEFAULT 'UNCLASSIFIED',
                    Status TEXT NOT NULL DEFAULT 'OPEN',
                    ThicknessChecked INTEGER NOT NULL DEFAULT 0,
                    NominalThicknessMm REAL NOT NULL DEFAULT 0,
                    ActualThicknessMm REAL NOT NULL DEFAULT 0,
                    MinimumThicknessMm REAL NOT NULL DEFAULT 0,
                    ThicknessStatus TEXT NOT NULL DEFAULT 'NOT CHECKED',
                    ThicknessRemark TEXT NOT NULL DEFAULT '',
                    CreatedBy TEXT NOT NULL DEFAULT '',
                    CreatedOn TEXT NOT NULL,
                    UpdatedBy TEXT NOT NULL DEFAULT '',
                    UpdatedOn TEXT NULL
                );
                """;

            createCommand.ExecuteNonQuery();

            var requiredColumns =
                new Dictionary<string, string>(
                    StringComparer.OrdinalIgnoreCase)
                {
                    ["LengthMm"] =
                        "REAL NOT NULL DEFAULT 0",

                    ["WidthMm"] =
                        "REAL NOT NULL DEFAULT 0",

                    ["ThicknessChecked"] =
                        "INTEGER NOT NULL DEFAULT 0",

                    ["NominalThicknessMm"] =
                        "REAL NOT NULL DEFAULT 0",

                    ["ActualThicknessMm"] =
                        "REAL NOT NULL DEFAULT 0",

                    ["MinimumThicknessMm"] =
                        "REAL NOT NULL DEFAULT 0",

                    ["ThicknessStatus"] =
                        "TEXT NOT NULL DEFAULT 'NOT CHECKED'",

                    ["ThicknessRemark"] =
                        "TEXT NOT NULL DEFAULT ''"
                };

            var existingColumns =
                new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase);

            using var pragmaCommand =
                connection.CreateCommand();

            pragmaCommand.CommandText =
                "PRAGMA table_info(Defects);";

            using var reader =
                pragmaCommand.ExecuteReader();

            while (reader.Read())
            {
                existingColumns.Add(
                    reader.GetString(1));
            }

            foreach (var column in requiredColumns)
            {
                if (existingColumns.Contains(
                    column.Key))
                {
                    continue;
                }

                using var alterCommand =
                    connection.CreateCommand();

                alterCommand.CommandText =
                    $"ALTER TABLE Defects " +
                    $"ADD COLUMN [{column.Key}] " +
                    $"{column.Value};";

                alterCommand.ExecuteNonQuery();
            }
        }
        finally
        {
            if (shouldClose)
            {
                connection.Close();
            }
        }
    }
}