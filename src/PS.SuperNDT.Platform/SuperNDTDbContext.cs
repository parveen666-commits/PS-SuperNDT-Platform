using Microsoft.EntityFrameworkCore;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Database;

public sealed class SuperNDTDbContext : DbContext
{
    public DbSet<JobModel> Jobs =>
        Set<JobModel>();

    public DbSet<ImageRecordModel> Images =>
        Set<ImageRecordModel>();

    public DbSet<CustomerModel> Customers =>
        Set<CustomerModel>();

    public DbSet<ShotPlanModel> ShotPlans =>
        Set<ShotPlanModel>();

    public DbSet<ShotPlanItemModel> ShotPlanItems =>
        Set<ShotPlanItemModel>();

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

        modelBuilder.Entity<ShotPlanModel>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.PipeId)
                .HasMaxLength(100);

            entity.Property(e => e.WeldNumber)
                .HasMaxLength(100);

            entity.Property(e => e.Direction)
                .HasMaxLength(50);

            entity.Property(e => e.AcquisitionMode)
                .HasMaxLength(50);

            entity.Property(e => e.Status)
                .HasMaxLength(50);

            entity.Ignore(e => e.StepLengthMm);
            entity.Ignore(e => e.CurrentShot);
            entity.Ignore(e => e.CompletedShotCount);
            entity.Ignore(e => e.AcceptedShotCount);
            entity.Ignore(e => e.ReviewedShotCount);
            entity.Ignore(e => e.ProgressPercentage);
            entity.Ignore(e => e.HasShots);
            entity.Ignore(e => e.HasCurrentShot);
            entity.Ignore(e => e.Shots);
        });

        modelBuilder.Entity<ShotPlanItemModel>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.PipeId)
                .HasMaxLength(100);

            entity.Property(e => e.WeldNumber)
                .HasMaxLength(100);

            entity.Property(e => e.Status)
                .HasMaxLength(50);

            entity.Property(e => e.AcquisitionMode)
                .HasMaxLength(50);

            entity.Property(e => e.ImageFileName)
                .HasMaxLength(300);

            entity.Property(e => e.Remarks)
                .HasMaxLength(1000);

            entity.Ignore(e => e.PositionText);
            entity.Ignore(e => e.RulerText);
        });

        base.OnModelCreating(modelBuilder);
    }

    public void Initialize()
    {
        Database.EnsureCreated();
    }
}