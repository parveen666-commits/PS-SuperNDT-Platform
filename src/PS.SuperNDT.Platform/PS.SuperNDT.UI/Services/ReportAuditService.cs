using System;
using System.Collections.Generic;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportAuditService
{
    private readonly List<ReportAuditModel> _auditLogs = new();


    public IReadOnlyList<ReportAuditModel> GetAll()
    {
        return _auditLogs;
    }


    public void Record(
        Guid reportId,
        string reportNumber,
        string action,
        string description,
        string performedBy)
    {
        _auditLogs.Add(
            new ReportAuditModel
            {
                Id = Guid.NewGuid(),

                ReportId = reportId,

                ReportNumber = reportNumber,

                Action = action,

                Description = description,

                PerformedBy = performedBy,

                PerformedOn = DateTime.Now
            });
    }


    public void Clear()
    {
        _auditLogs.Clear();
    }
}