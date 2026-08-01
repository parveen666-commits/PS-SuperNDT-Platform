using System;

namespace PS.SuperNDT.UI.Models;

public sealed class ReportSignatureModel
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid ReportId { get; set; }

    public string ReportNumber { get; set; } = string.Empty;


    public string SignedBy { get; set; } = string.Empty;

    public string SignerName
    {
        get => SignedBy;
        set => SignedBy = value;
    }


    public string Designation { get; set; } = string.Empty;


    public string SignatureType { get; set; } = string.Empty;


    public string SignatureData { get; set; } = string.Empty;


    public DateTime SignedOn { get; set; } = DateTime.Now;


    public bool IsValid { get; set; }


    public bool IsVerified
    {
        get => IsValid;
        set => IsValid = value;
    }


    public string Remarks { get; set; } = string.Empty;
}