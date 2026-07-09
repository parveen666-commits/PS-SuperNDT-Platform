using Microsoft.EntityFrameworkCore;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Database;

public sealed class SuperNDTDbContext : DbContext
{
    public DbSet<JobModel> Jobs => Set<JobModel>();

    public DbSet<ImageRecordModel> Images => Set<ImageRecordModel>();


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
    }
}