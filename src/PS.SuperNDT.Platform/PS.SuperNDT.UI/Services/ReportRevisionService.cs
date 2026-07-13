using System;
using System.Collections.ObjectModel;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportRevisionService
{
    private readonly ObservableCollection<ReportRevisionModel> _revisions;

    public ReportRevisionService()
    {
        _revisions = new ObservableCollection<ReportRevisionModel>();
    }

    public ReadOnlyObservableCollection<ReportRevisionModel> Revisions =>
        new(_revisions);

    public ReportRevisionModel CreateRevision(
        Guid reportId,
        string reportNumber,
        string changeDescription,
        string revisedBy)
    {
        foreach (var revision in _revisions)
        {
            revision.IsCurrentRevision = false;
        }

        var newRevision = new ReportRevisionModel
        {
            ReportId = reportId,
            ReportNumber = reportNumber,
            RevisionNumber = GenerateRevisionNumber(),
            ChangeDescription = changeDescription,
            RevisedBy = revisedBy,
            RevisedOn = DateTime.Now,
            IsCurrentRevision = true
        };

        _revisions.Add(newRevision);

        return newRevision;
    }

    private string GenerateRevisionNumber()
    {
        return $"Rev-{_revisions.Count:00}";
    }
}