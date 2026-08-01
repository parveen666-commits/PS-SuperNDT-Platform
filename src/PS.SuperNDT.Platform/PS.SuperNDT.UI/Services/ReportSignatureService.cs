using System;
using System.Collections.Generic;
using System.Linq;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportSignatureService
{
    private readonly List<ReportSignatureModel> _signatures = new();


    public IReadOnlyList<ReportSignatureModel> GetAll()
    {
        return _signatures;
    }


    public IEnumerable<ReportSignatureModel> GetByReport(
        Guid reportId)
    {
        return _signatures
            .Where(x => x.ReportId == reportId)
            .OrderByDescending(x => x.SignedOn);
    }


    public void Add(
        ReportSignatureModel signature)
    {
        ArgumentNullException.ThrowIfNull(signature);


        if (signature.Id == Guid.Empty)
        {
            signature.Id =
                Guid.NewGuid();
        }


        signature.SignedOn =
            DateTime.Now;


        _signatures.Add(signature);
    }


    public void Verify(
        Guid signatureId)
    {
        var signature =
            _signatures.FirstOrDefault(
                x => x.Id == signatureId);


        if (signature == null)
            return;


        signature.IsValid = true;
    }


    public void Remove(
        Guid signatureId)
    {
        var signature =
            _signatures.FirstOrDefault(
                x => x.Id == signatureId);


        if (signature != null)
        {
            _signatures.Remove(signature);
        }
    }


    public void Clear()
    {
        _signatures.Clear();
    }
}