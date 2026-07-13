using System;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportProcedureService
{
    public ReportProcedureModel Create(
        Guid reportId,
        string procedureNumber,
        string procedureTitle,
        string revision,
        string standardReference,
        string preparedBy)
    {
        return new ReportProcedureModel
        {
            ReportId = reportId,
            ProcedureNumber = procedureNumber,
            ProcedureTitle = procedureTitle,
            Revision = revision,
            StandardReference = standardReference,
            PreparedBy = preparedBy,
            ApprovedDate = DateTime.Now
        };
    }

    public bool Validate(
        ReportProcedureModel procedure)
    {
        if (procedure == null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(procedure.ProcedureNumber))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(procedure.StandardReference))
        {
            return false;
        }

        return true;
    }
}