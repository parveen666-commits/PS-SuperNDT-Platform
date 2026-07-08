using Microsoft.EntityFrameworkCore;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Database;

public sealed class SuperNDTDbContext : DbContext
{
    public DbSet<JobModel> Jobs => Set<JobModel>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=PS_SuperNDT.db");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
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

        base.OnModelCreating(modelBuilder);
    }

    public void Initialize()
    {
        Database.EnsureCreated();
    }
}