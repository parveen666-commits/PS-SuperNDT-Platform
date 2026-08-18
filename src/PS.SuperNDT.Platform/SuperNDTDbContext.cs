using System;
using System.Collections.Generic;
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

            entity.Property(e => e.Remarks)
                  .HasMaxLength(1000);
        });

        base.OnModelCreating(modelBuilder);
    }

    public void Initialize()
    {
        Database.EnsureCreated();

        EnsureImagePipeIdColumn();
    }

    private void EnsureImagePipeIdColumn()
    {
        try
        {
            using var connection =
                new SqliteConnection(
                    "Data Source=PS_SuperNDT.db");

            connection.Open();

            var existingColumns =
                new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase);

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

            if (!existingColumns.Contains(
                    "PipeId"))
            {
                using var command =
                    connection.CreateCommand();

                command.CommandText =
                    "ALTER TABLE Images " +
                    "ADD COLUMN PipeId TEXT NOT NULL DEFAULT '';";

                command.ExecuteNonQuery();
            }
        }
        catch
        {
            // Existing database compatibility should
            // never prevent application startup.
        }
    }
}