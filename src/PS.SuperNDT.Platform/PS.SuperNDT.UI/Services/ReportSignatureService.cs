using System;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportSignatureService
{
    public ReportSignatureModel CreateSignature(
        Guid reportId,
        string signedBy,
        string designation,
        string signatureType)
    {
        return new ReportSignatureModel
        {
            ReportId = reportId,
            SignedBy = signedBy,
            Designation = designation,
            SignatureType = signatureType,
            SignatureData = GenerateSignatureToken(),
            SignedOn = DateTime.Now,
            IsValid = true
        };
    }

    public bool ValidateSignature(
        ReportSignatureModel signature)
    {
        if (signature == null)
        {
            return false;
        }

        return signature.IsValid &&
               !string.IsNullOrWhiteSpace(signature.SignatureData);
    }

    private string GenerateSignatureToken()
    {
        return Guid.NewGuid()
            .ToString()
            .Replace("-", "")
            .ToUpperInvariant();
    }
}