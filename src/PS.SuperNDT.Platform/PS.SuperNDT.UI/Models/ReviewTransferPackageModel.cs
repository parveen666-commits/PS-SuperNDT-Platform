using System;
using System.Collections.Generic;

namespace PS.SuperNDT.UI.Models;

public sealed class ReviewTransferPackageModel
{
    public Guid Id { get; set; }

    public string PackageNumber { get; set; } = string.Empty;

    public string JobNumber { get; set; } = string.Empty;

    public string InspectionNumber { get; set; } = string.Empty;

    public string Customer { get; set; } = string.Empty;

    public string Project { get; set; } = string.Empty;

    public string Component { get; set; } = string.Empty;

    public string WeldNumber { get; set; } = string.Empty;

    public string Technique { get; set; } = string.Empty;

    public string Operator { get; set; } = string.Empty;

    public DateTime CreatedOn { get; set; }

    public DateTime TransferDate { get; set; }

    public string TransferDestination { get; set; } = string.Empty;

    public int ImageCount { get; set; }

    public long PackageSizeBytes { get; set; }

    public List<string> ImageFiles { get; set; } = new();

    public TransferStatus Status { get; set; }

    public bool ReadyForReview { get; set; }

    public string Notes { get; set; } = string.Empty;
}