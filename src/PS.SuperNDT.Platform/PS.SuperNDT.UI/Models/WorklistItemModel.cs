using System;

namespace PS.SuperNDT.UI.Models;

public sealed class WorklistItemModel
{
    public Guid Id { get; set; }

    public string JobNumber { get; set; } = string.Empty;

    public string InspectionNumber { get; set; } = string.Empty;

    public string Customer { get; set; } = string.Empty;

    public string Project { get; set; } = string.Empty;

    public string Component { get; set; } = string.Empty;

    public string WeldNumber { get; set; } = string.Empty;

    public string Operator { get; set; } = string.Empty;

    public string Technique { get; set; } = string.Empty;

    public int ImageCount { get; set; }

    public DateTime CreatedOn { get; set; }

    public bool ReadyForTransfer { get; set; }

    public TransferStatus TransferStatus { get; set; }

    public string Remarks { get; set; } = string.Empty;
}