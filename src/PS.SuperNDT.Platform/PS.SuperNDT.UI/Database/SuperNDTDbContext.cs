using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Database;

public sealed class SuperNDTDbContext : DbContext
{
    public DbSet<JobModel> Jobs => Set<JobModel>();

    public DbSet<WorkOrderModel> WorkOrders => Set<WorkOrderModel>();

    public DbSet<ImageRecordModel> Images => Set<ImageRecordModel>();

    public DbSet<CustomerModel> Customers => Set<CustomerModel>();

    public DbSet<DefectModel> Defects => Set<DefectModel>();

    public DbSet<WeldModel> Welds => Set<WeldModel>();

    public DbSet<PipeModel> Pipes => Set<PipeModel>();

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
        // ========================================================
        // JOB
        // ========================================================

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

        // ========================================================
        // WORK ORDER
        // ========================================================

        modelBuilder.Entity<WorkOrderModel>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.WorkOrderNumber)
                .HasMaxLength(100);

            entity.Property(e => e.Customer)
                .HasMaxLength(200);

            entity.Property(e => e.Project)
                .HasMaxLength(200);

            entity.Property(e => e.Component)
                .HasMaxLength(200);

            entity.Property(e => e.DrawingNumber)
                .HasMaxLength(200);

            entity.Property(e => e.PurchaseOrder)
                .HasMaxLength(200);

            entity.Property(e => e.Procedure)
                .HasMaxLength(200);

            entity.Property(e => e.Technique)
                .HasMaxLength(200);

            entity.Property(e => e.Material)
                .HasMaxLength(100);

            entity.Property(e => e.MaterialSpecification)
                .HasMaxLength(200);

            entity.Property(e => e.InspectionStandard)
                .HasMaxLength(200);

            entity.Property(e => e.AcceptanceStandard)
                .HasMaxLength(200);

            entity.Property(e => e.Status)
                .HasMaxLength(50);

            entity.Property(e => e.Result)
                .HasMaxLength(50);

            entity.Property(e => e.AssignedOperator)
                .HasMaxLength(100);

            entity.Property(e => e.AssignedInspector)
                .HasMaxLength(100);

            entity.Property(e => e.StoragePath)
                .HasMaxLength(1000);

            entity.Property(e => e.ReviewPath)
                .HasMaxLength(1000);

            entity.Property(e => e.DiconPath)
                .HasMaxLength(1000);

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100);

            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100);

            entity.Property(e => e.Remark)
                .HasMaxLength(1000);

            entity.Property(e => e.InternalRemark)
                .HasMaxLength(2000);

            entity.HasIndex(e => e.WorkOrderNumber);

            entity.HasIndex(e => e.Status);

            entity.HasIndex(e => e.Result);
        });

        // ========================================================
        // IMAGE / SHOT
        // ========================================================

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

        // ========================================================
        // WELD
        // ========================================================

        modelBuilder.Entity<WeldModel>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.WeldNumber)
                .HasMaxLength(100);

            entity.Property(e => e.SpoolNumber)
                .HasMaxLength(100);

            entity.Property(e => e.LineNumber)
                .HasMaxLength(100);

            entity.Property(e => e.JointType)
                .HasMaxLength(100);

            entity.Property(e => e.Material)
                .HasMaxLength(100);

            entity.Property(e => e.Schedule)
                .HasMaxLength(100);

            entity.Property(e => e.Technique)
                .HasMaxLength(200);

            entity.Property(e => e.InspectionStatus)
                .HasMaxLength(50);

            entity.Property(e => e.Remarks)
                .HasMaxLength(1000);
        });

        // ========================================================
        // PIPE
        // ========================================================

        modelBuilder.Entity<PipeModel>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.WorkOrderId)
                .IsRequired();

            entity.Property(e => e.PipeNumber)
                .HasMaxLength(100);

            entity.Property(e => e.HeatNumber)
                .HasMaxLength(100);

            entity.Property(e => e.Material)
                .HasMaxLength(100);

            entity.Property(e => e.MaterialSpecification)
                .HasMaxLength(200);

            entity.Property(e => e.DrawingNumber)
                .HasMaxLength(200);

            entity.Property(e => e.BatchNumber)
                .HasMaxLength(100);

            entity.Property(e => e.Status)
                .HasMaxLength(50);

            entity.Property(e => e.Result)
                .HasMaxLength(50);

            entity.Property(e => e.RepairLocation)
                .HasMaxLength(500);

            entity.Property(e => e.RepairRemark)
                .HasMaxLength(1000);

            entity.Property(e => e.Operator)
                .HasMaxLength(100);

            entity.Property(e => e.Inspector)
                .HasMaxLength(100);

            entity.Property(e => e.StoragePath)
                .HasMaxLength(1000);

            entity.Property(e => e.ReviewPath)
                .HasMaxLength(1000);

            entity.Property(e => e.DiconPath)
                .HasMaxLength(1000);

            entity.Property(e => e.CreatedBy)
                .HasMaxLength(100);

            entity.Property(e => e.UpdatedBy)
                .HasMaxLength(100);

            entity.Property(e => e.Remark)
                .HasMaxLength(1000);

            entity.HasIndex(e => e.WorkOrderId);

            entity.HasIndex(e => e.PipeNumber);

            entity.HasIndex(e => e.Status);

            entity.HasIndex(e => e.Result);
        });

        // ========================================================
        // DEFECT
        // ========================================================

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

    // ============================================================
    // DATABASE INITIALIZATION
    // ============================================================

    public void Initialize()
    {
        Database.EnsureCreated();

        EnsureWorkOrderTableSchema();

        EnsureImageTableSchema();

        EnsureDefectTableSchema();

        EnsureWeldTableSchema();

        EnsurePipeTableSchema();
    }

    // ============================================================
    // WORK ORDER TABLE
    // ============================================================

    private void EnsureWorkOrderTableSchema()
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
                CREATE TABLE IF NOT EXISTS WorkOrders
                (
                    Id TEXT NOT NULL PRIMARY KEY,

                    WorkOrderNumber TEXT NOT NULL DEFAULT '',

                    Customer TEXT NOT NULL DEFAULT '',

                    Project TEXT NOT NULL DEFAULT '',

                    Component TEXT NOT NULL DEFAULT '',

                    DrawingNumber TEXT NOT NULL DEFAULT '',

                    PurchaseOrder TEXT NOT NULL DEFAULT '',

                    Procedure TEXT NOT NULL DEFAULT '',

                    Technique TEXT NOT NULL DEFAULT '',

                    Material TEXT NOT NULL DEFAULT '',

                    NominalThicknessMm REAL NOT NULL DEFAULT 0,

                    MaterialSpecification TEXT NOT NULL DEFAULT '',

                    InspectionStandard TEXT NOT NULL DEFAULT '',

                    AcceptanceStandard TEXT NOT NULL DEFAULT '',

                    PipeDiameterMm REAL NOT NULL DEFAULT 0,

                    PipeLengthMm REAL NOT NULL DEFAULT 0,

                    DefaultShotSizeMm REAL NOT NULL DEFAULT 0,

                    DefaultOverlapPercent REAL NOT NULL DEFAULT 0,

                    Status TEXT NOT NULL DEFAULT 'CREATED',

                    Result TEXT NOT NULL DEFAULT 'PENDING',

                    AssignedOperator TEXT NOT NULL DEFAULT '',

                    AssignedInspector TEXT NOT NULL DEFAULT '',

                    TotalPipes INTEGER NOT NULL DEFAULT 0,

                    TotalShots INTEGER NOT NULL DEFAULT 0,

                    CompletedShots INTEGER NOT NULL DEFAULT 0,

                    PendingShots INTEGER NOT NULL DEFAULT 0,

                    RepairPipes INTEGER NOT NULL DEFAULT 0,

                    AcceptedPipes INTEGER NOT NULL DEFAULT 0,

                    RejectedPipes INTEGER NOT NULL DEFAULT 0,

                    StoragePath TEXT NOT NULL DEFAULT '',

                    ReviewPath TEXT NOT NULL DEFAULT '',

                    DiconPath TEXT NOT NULL DEFAULT '',

                    CreatedOn TEXT NOT NULL,

                    StartedOn TEXT NULL,

                    CompletedOn TEXT NULL,

                    ClosedOn TEXT NULL,

                    CreatedBy TEXT NOT NULL DEFAULT '',

                    UpdatedBy TEXT NOT NULL DEFAULT '',

                    UpdatedOn TEXT NULL,

                    Remark TEXT NOT NULL DEFAULT '',

                    InternalRemark TEXT NOT NULL DEFAULT ''
                );
                """;

            createCommand.ExecuteNonQuery();

            // ====================================================
            // MIGRATION FOR EXISTING DATABASE
            // ====================================================

            var requiredColumns =
                new Dictionary<string, string>(
                    StringComparer.OrdinalIgnoreCase)
                {
                    ["WorkOrderNumber"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["Customer"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["Project"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["Component"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["DrawingNumber"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["PurchaseOrder"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["Procedure"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["Technique"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["Material"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["NominalThicknessMm"] =
                        "REAL NOT NULL DEFAULT 0",

                    ["MaterialSpecification"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["InspectionStandard"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["AcceptanceStandard"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["PipeDiameterMm"] =
                        "REAL NOT NULL DEFAULT 0",

                    ["PipeLengthMm"] =
                        "REAL NOT NULL DEFAULT 0",

                    ["DefaultShotSizeMm"] =
                        "REAL NOT NULL DEFAULT 0",

                    ["DefaultOverlapPercent"] =
                        "REAL NOT NULL DEFAULT 0",

                    ["Status"] =
                        "TEXT NOT NULL DEFAULT 'CREATED'",

                    ["Result"] =
                        "TEXT NOT NULL DEFAULT 'PENDING'",

                    ["AssignedOperator"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["AssignedInspector"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["TotalPipes"] =
                        "INTEGER NOT NULL DEFAULT 0",

                    ["TotalShots"] =
                        "INTEGER NOT NULL DEFAULT 0",

                    ["CompletedShots"] =
                        "INTEGER NOT NULL DEFAULT 0",

                    ["PendingShots"] =
                        "INTEGER NOT NULL DEFAULT 0",

                    ["RepairPipes"] =
                        "INTEGER NOT NULL DEFAULT 0",

                    ["AcceptedPipes"] =
                        "INTEGER NOT NULL DEFAULT 0",

                    ["RejectedPipes"] =
                        "INTEGER NOT NULL DEFAULT 0",

                    ["StoragePath"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["ReviewPath"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["DiconPath"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["CreatedOn"] =
                        "TEXT NOT NULL",

                    ["StartedOn"] =
                        "TEXT NULL",

                    ["CompletedOn"] =
                        "TEXT NULL",

                    ["ClosedOn"] =
                        "TEXT NULL",

                    ["CreatedBy"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["UpdatedBy"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["UpdatedOn"] =
                        "TEXT NULL",

                    ["Remark"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["InternalRemark"] =
                        "TEXT NOT NULL DEFAULT ''"
                };

            var existingColumns =
                new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase);

            using var pragmaCommand =
                connection.CreateCommand();

            pragmaCommand.CommandText =
                "PRAGMA table_info(WorkOrders);";

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
                    $"ALTER TABLE WorkOrders " +
                    $"ADD COLUMN [{column.Key}] " +
                    $"{column.Value};";

                alterCommand.ExecuteNonQuery();
            }

            // ====================================================
            // INDEXES
            // ====================================================

            using var indexCommand =
                connection.CreateCommand();

            indexCommand.CommandText =
                """
                CREATE INDEX IF NOT EXISTS
                IX_WorkOrders_WorkOrderNumber
                ON WorkOrders(WorkOrderNumber);

                CREATE INDEX IF NOT EXISTS
                IX_WorkOrders_Status
                ON WorkOrders(Status);

                CREATE INDEX IF NOT EXISTS
                IX_WorkOrders_Result
                ON WorkOrders(Result);
                """;

            indexCommand.ExecuteNonQuery();
        }
        finally
        {
            if (shouldClose)
            {
                connection.Close();
            }
        }
    }

    // ============================================================
    // IMAGE TABLE
    // ============================================================

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

    // ============================================================
    // DEFECT TABLE
    // ============================================================

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

    // ============================================================
    // WELD TABLE
    // ============================================================

    private void EnsureWeldTableSchema()
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
                CREATE TABLE IF NOT EXISTS Welds
                (
                    Id TEXT NOT NULL PRIMARY KEY,
                    JobId TEXT NOT NULL,
                    WeldNumber TEXT NOT NULL DEFAULT '',
                    SpoolNumber TEXT NOT NULL DEFAULT '',
                    LineNumber TEXT NOT NULL DEFAULT '',
                    JointType TEXT NOT NULL DEFAULT '',
                    Material TEXT NOT NULL DEFAULT '',
                    Diameter REAL NOT NULL DEFAULT 0,
                    Thickness REAL NOT NULL DEFAULT 0,
                    Schedule TEXT NOT NULL DEFAULT '',
                    Technique TEXT NOT NULL DEFAULT '',
                    InspectionStatus TEXT NOT NULL DEFAULT 'Pending',
                    TotalShots INTEGER NOT NULL DEFAULT 0,
                    AcceptedShots INTEGER NOT NULL DEFAULT 0,
                    RejectedShots INTEGER NOT NULL DEFAULT 0,
                    Remarks TEXT NOT NULL DEFAULT '',
                    CreatedOn TEXT NOT NULL
                );
                """;

            createCommand.ExecuteNonQuery();
        }
        finally
        {
            if (shouldClose)
            {
                connection.Close();
            }
        }
    }

    // ============================================================
    // PIPE TABLE
    // ============================================================

    private void EnsurePipeTableSchema()
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
                CREATE TABLE IF NOT EXISTS Pipes
                (
                    Id TEXT NOT NULL PRIMARY KEY,

                    WorkOrderId TEXT NOT NULL,

                    PipeNumber TEXT NOT NULL DEFAULT '',

                    HeatNumber TEXT NOT NULL DEFAULT '',

                    Material TEXT NOT NULL DEFAULT '',

                    MaterialSpecification TEXT NOT NULL DEFAULT '',

                    DiameterMm REAL NOT NULL DEFAULT 0,

                    LengthMm REAL NOT NULL DEFAULT 0,

                    ThicknessMm REAL NOT NULL DEFAULT 0,

                    DrawingNumber TEXT NOT NULL DEFAULT '',

                    BatchNumber TEXT NOT NULL DEFAULT '',

                    TotalWelds INTEGER NOT NULL DEFAULT 0,

                    CompletedWelds INTEGER NOT NULL DEFAULT 0,

                    TotalShots INTEGER NOT NULL DEFAULT 0,

                    CompletedShots INTEGER NOT NULL DEFAULT 0,

                    PendingShots INTEGER NOT NULL DEFAULT 0,

                    DefectShots INTEGER NOT NULL DEFAULT 0,

                    Status TEXT NOT NULL DEFAULT 'PENDING',

                    Result TEXT NOT NULL DEFAULT 'PENDING',

                    RequiresRepair INTEGER NOT NULL DEFAULT 0,

                    RepairLocation TEXT NOT NULL DEFAULT '',

                    RepairRemark TEXT NOT NULL DEFAULT '',

                    RepairCycle INTEGER NOT NULL DEFAULT 0,

                    Operator TEXT NOT NULL DEFAULT '',

                    Inspector TEXT NOT NULL DEFAULT '',

                    StoragePath TEXT NOT NULL DEFAULT '',

                    ReviewPath TEXT NOT NULL DEFAULT '',

                    DiconPath TEXT NOT NULL DEFAULT '',

                    CreatedOn TEXT NOT NULL,

                    RTStartedOn TEXT NULL,

                    RTCompletedOn TEXT NULL,

                    ReviewStartedOn TEXT NULL,

                    ReviewedOn TEXT NULL,

                    AcceptedOn TEXT NULL,

                    ClosedOn TEXT NULL,

                    CreatedBy TEXT NOT NULL DEFAULT '',

                    UpdatedBy TEXT NOT NULL DEFAULT '',

                    UpdatedOn TEXT NULL,

                    Remark TEXT NOT NULL DEFAULT ''
                );
                """;

            createCommand.ExecuteNonQuery();

            var requiredColumns =
                new Dictionary<string, string>(
                    StringComparer.OrdinalIgnoreCase)
                {
                    ["WorkOrderId"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["PipeNumber"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["HeatNumber"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["Material"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["MaterialSpecification"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["DiameterMm"] =
                        "REAL NOT NULL DEFAULT 0",

                    ["LengthMm"] =
                        "REAL NOT NULL DEFAULT 0",

                    ["ThicknessMm"] =
                        "REAL NOT NULL DEFAULT 0",

                    ["DrawingNumber"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["BatchNumber"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["TotalWelds"] =
                        "INTEGER NOT NULL DEFAULT 0",

                    ["CompletedWelds"] =
                        "INTEGER NOT NULL DEFAULT 0",

                    ["TotalShots"] =
                        "INTEGER NOT NULL DEFAULT 0",

                    ["CompletedShots"] =
                        "INTEGER NOT NULL DEFAULT 0",

                    ["PendingShots"] =
                        "INTEGER NOT NULL DEFAULT 0",

                    ["DefectShots"] =
                        "INTEGER NOT NULL DEFAULT 0",

                    ["Status"] =
                        "TEXT NOT NULL DEFAULT 'PENDING'",

                    ["Result"] =
                        "TEXT NOT NULL DEFAULT 'PENDING'",

                    ["RequiresRepair"] =
                        "INTEGER NOT NULL DEFAULT 0",

                    ["RepairLocation"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["RepairRemark"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["RepairCycle"] =
                        "INTEGER NOT NULL DEFAULT 0",

                    ["Operator"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["Inspector"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["StoragePath"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["ReviewPath"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["DiconPath"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["CreatedOn"] =
                        "TEXT NOT NULL",

                    ["RTStartedOn"] =
                        "TEXT NULL",

                    ["RTCompletedOn"] =
                        "TEXT NULL",

                    ["ReviewStartedOn"] =
                        "TEXT NULL",

                    ["ReviewedOn"] =
                        "TEXT NULL",

                    ["AcceptedOn"] =
                        "TEXT NULL",

                    ["ClosedOn"] =
                        "TEXT NULL",

                    ["CreatedBy"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["UpdatedBy"] =
                        "TEXT NOT NULL DEFAULT ''",

                    ["UpdatedOn"] =
                        "TEXT NULL",

                    ["Remark"] =
                        "TEXT NOT NULL DEFAULT ''"
                };

            var existingColumns =
                new HashSet<string>(
                    StringComparer.OrdinalIgnoreCase);

            using var pragmaCommand =
                connection.CreateCommand();

            pragmaCommand.CommandText =
                "PRAGMA table_info(Pipes);";

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
                    $"ALTER TABLE Pipes " +
                    $"ADD COLUMN [{column.Key}] " +
                    $"{column.Value};";

                alterCommand.ExecuteNonQuery();
            }

            // ====================================================
            // INDEXES
            // ====================================================

            using var indexCommand =
                connection.CreateCommand();

            indexCommand.CommandText =
                """
                CREATE INDEX IF NOT EXISTS
                IX_Pipes_WorkOrderId
                ON Pipes(WorkOrderId);

                CREATE INDEX IF NOT EXISTS
                IX_Pipes_PipeNumber
                ON Pipes(PipeNumber);

                CREATE INDEX IF NOT EXISTS
                IX_Pipes_Status
                ON Pipes(Status);

                CREATE INDEX IF NOT EXISTS
                IX_Pipes_Result
                ON Pipes(Result);
                """;

            indexCommand.ExecuteNonQuery();
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