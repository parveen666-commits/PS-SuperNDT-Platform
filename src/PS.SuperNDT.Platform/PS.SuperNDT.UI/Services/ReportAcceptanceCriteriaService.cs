using System;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportAcceptanceCriteriaService
{
    public ReportAcceptanceCriteriaModel Create(
        Guid reportId,
        string standard,
        string acceptanceLevel,
        string criteriaDescription)
    {
        return new ReportAcceptanceCriteriaModel
        {
            ReportId = reportId,
            Standard = standard,
            AcceptanceLevel = acceptanceLevel,
            CriteriaDescription = criteriaDescription,
            IsAccepted = false
        };
    }

    public void Evaluate(
        ReportAcceptanceCriteriaModel criteria,
        bool accepted,
        string evaluatedBy,
        string remarks)
    {
        if (criteria == null)
        {
            return;
        }

        criteria.IsAccepted = accepted;
        criteria.EvaluatedBy = evaluatedBy;
        criteria.Remarks = remarks;
        criteria.EvaluatedOn = DateTime.Now;
    }

    public string GetResult(
        ReportAcceptanceCriteriaModel criteria)
    {
        if (criteria == null)
        {
            return "Not Evaluated";
        }

        return criteria.IsAccepted
            ? "Accepted"
            : "Rejected";
    }
}