using Microsoft.EntityFrameworkCore;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Database;

public partial class SuperNDTDbContext : DbContext
{
    public DbSet<ReportDataModel> Reports => Set<ReportDataModel>();

    public DbSet<ReportFindingModel> ReportFindings => Set<ReportFindingModel>();

    public DbSet<ReportImageModel> ReportImages => Set<ReportImageModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ReportDataModel>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<ReportFindingModel>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<ReportImageModel>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<ReportDataModel>()
            .HasMany(x => x.Findings)
            .WithOne()
            .HasForeignKey(x => x.ReportId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ReportDataModel>()
            .HasMany(x => x.Images)
            .WithOne()
            .HasForeignKey(x => x.ReportId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}