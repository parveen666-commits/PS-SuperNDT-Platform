using System;
using System.Collections.Generic;

namespace PS.SuperNDT.UI.Models;

public sealed class InspectionPackageModel
{
    public Guid Id { get; set; }

    public string PackageNumber { get; set; } = string.Empty;

    public string JobNumber { get; set; } = string.Empty;

    public string InspectionNumber { get; set; } = string.Empty;

    public string Customer { get; set; } = string.Empty;

    public string Project { get; set; } = string.Empty;

    public string Component { get; set; } = string.Empty;

    public string WeldNumber { get; set; } = string.Empty;

    public string Operator { get; set; } = string.Empty;

    public DateTime CreatedOn { get; set; }

    public string Source { get; set; } = string.Empty;

    public string Detector { get; set; } = string.Empty;

    public string Technique { get; set; } = string.Empty;

    public List<string> ImageFiles { get; set; } = new();

    public long TotalSizeBytes { get; set; }

    public TransferStatus TransferStatus { get; set; }

    public bool ApprovedForTransfer { get; set; }

    public string Notes { get; set; } = string.Empty;
}