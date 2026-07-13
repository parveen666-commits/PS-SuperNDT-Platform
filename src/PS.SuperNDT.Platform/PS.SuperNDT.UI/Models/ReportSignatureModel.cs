using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportSignatureModel
{
    public Guid Id { get; set; }

    public Guid ReportId { get; set; }

    public string SignatureType { get; set; } = string.Empty;

    public string SignerName { get; set; } = string.Empty;

    public string Designation { get; set; } = string.Empty;

    public string SignatureData { get; set; } = string.Empty;

    public string SignatureImagePath { get; set; } = string.Empty;

    public bool IsValid { get; set; }

    public bool IsApproved { get; set; }

    public string Remarks { get; set; } = string.Empty;

    public DateTime SignedOn { get; set; }

    public string SignedBy { get; set; } = string.Empty;
}