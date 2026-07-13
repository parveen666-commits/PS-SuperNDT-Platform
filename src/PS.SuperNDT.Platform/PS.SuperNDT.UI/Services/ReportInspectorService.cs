using System;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportInspectorService
{
    public ReportInspectorModel Create(
        Guid reportId,
        string inspectorName,
        string inspectorLevel,
        string certificationNumber,
        string employer)
    {
        return new ReportInspectorModel
        {
            ReportId = reportId,
            InspectorName = inspectorName,
            InspectorLevel = inspectorLevel,
            CertificationNumber = certificationNumber,
            Employer = employer,
            InspectionDate = DateTime.Now
        };
    }

    public bool Validate(
        ReportInspectorModel inspector)
    {
        if (inspector == null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(inspector.InspectorName))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(inspector.CertificationNumber))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(inspector.InspectorLevel))
        {
            return false;
        }

        return true;
    }
}