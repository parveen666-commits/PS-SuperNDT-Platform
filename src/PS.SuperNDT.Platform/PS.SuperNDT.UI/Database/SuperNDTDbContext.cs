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

            entity.Property(e => e.Operator)
                  .HasMaxLength(100);

            entity.Property(e => e.FileName)
                  .HasMaxLength(300);

            entity.Property(e => e.FilePath)
                  .HasMaxLength(500);

            entity.Property(e => e.DetectorName)
                  .HasMaxLength(100);

            entity.Property(e => e.Remarks)
                  .HasMaxLength(1000);
        });


        base.OnModelCreating(modelBuilder);
    }


    public void Initialize()
    {
        Database.EnsureCreated();

        EnsureImageTableSchema();
    }


    private void EnsureImageTableSchema()
    {
        var requiredColumns =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
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
                if (existingColumns.Contains(column.Key))
                {
                    continue;
                }

                using var alterCommand =
                    connection.CreateCommand();

                alterCommand.CommandText =
                    $"ALTER TABLE Images ADD COLUMN [{column.Key}] {column.Value};";

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