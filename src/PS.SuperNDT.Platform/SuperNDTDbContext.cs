using Microsoft.Data.Sqlite;
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

            entity.Property(e => e.ReviewStatus)
                  .HasMaxLength(50);

            entity.Property(e => e.ReviewedBy)
                  .HasMaxLength(100);

            entity.Property(e => e.ShotNumber)
                  .HasDefaultValue(0);

            entity.Property(e => e.TotalShots)
                  .HasDefaultValue(0);

            entity.Property(e => e.PipeLength)
                  .HasDefaultValue(0);

            entity.Property(e => e.ShotSize)
                  .HasDefaultValue(0);

            entity.Property(e => e.Overlap)
                  .HasDefaultValue(0);

            entity.Property(e => e.ShotStartPosition)
                  .HasDefaultValue(0);

            entity.Property(e => e.ShotEndPosition)
                  .HasDefaultValue(0);
        });

        base.OnModelCreating(modelBuilder);
    }

    public void Initialize()
    {
        Database.EnsureCreated();

        EnsureImageShotColumns();
    }

    private void EnsureImageShotColumns()
    {
        using var connection =
            Database.GetDbConnection();

        var wasClosed =
            connection.State !=
            System.Data.ConnectionState.Open;

        if (wasClosed)
        {
            connection.Open();
        }

        try
        {
            var existingColumns =
                new HashSet<string>(
                    System.StringComparer.OrdinalIgnoreCase);

            using (
                var command =
                    connection.CreateCommand())
            {
                command.CommandText =
                    "PRAGMA table_info(Images);";

                using var reader =
                    command.ExecuteReader();

                while (reader.Read())
                {
                    existingColumns.Add(
                        reader.GetString(1));
                }
            }

            AddColumnIfMissing(
                connection,
                existingColumns,
                "ShotNumber",
                "INTEGER NOT NULL DEFAULT 0");

            AddColumnIfMissing(
                connection,
                existingColumns,
                "TotalShots",
                "INTEGER NOT NULL DEFAULT 0");

            AddColumnIfMissing(
                connection,
                existingColumns,
                "PipeLength",
                "REAL NOT NULL DEFAULT 0");

            AddColumnIfMissing(
                connection,
                existingColumns,
                "ShotSize",
                "REAL NOT NULL DEFAULT 0");

            AddColumnIfMissing(
                connection,
                existingColumns,
                "Overlap",
                "REAL NOT NULL DEFAULT 0");

            AddColumnIfMissing(
                connection,
                existingColumns,
                "ShotStartPosition",
                "REAL NOT NULL DEFAULT 0");

            AddColumnIfMissing(
                connection,
                existingColumns,
                "ShotEndPosition",
                "REAL NOT NULL DEFAULT 0");

            AddColumnIfMissing(
                connection,
                existingColumns,
                "ReviewStatus",
                "TEXT NOT NULL DEFAULT 'PENDING'");

            AddColumnIfMissing(
                connection,
                existingColumns,
                "ReviewedBy",
                "TEXT NOT NULL DEFAULT ''");

            AddColumnIfMissing(
                connection,
                existingColumns,
                "ReviewedOn",
                "TEXT NULL");
        }
        finally
        {
            if (wasClosed)
            {
                connection.Close();
            }
        }
    }

    private static void AddColumnIfMissing(
        System.Data.Common.DbConnection connection,
        HashSet<string> existingColumns,
        string columnName,
        string columnDefinition)
    {
        if (existingColumns.Contains(columnName))
        {
            return;
        }

        using var command =
            connection.CreateCommand();

        command.CommandText =
            $"ALTER TABLE Images " +
            $"ADD COLUMN {columnName} {columnDefinition};";

        command.ExecuteNonQuery();

        existingColumns.Add(columnName);
    }
}