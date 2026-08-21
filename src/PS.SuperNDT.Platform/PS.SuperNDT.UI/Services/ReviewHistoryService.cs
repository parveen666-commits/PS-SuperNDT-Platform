using System;
using System.Collections.Generic;
using System.Linq;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReviewHistoryService
{
    private readonly AuditLogService _auditLogService = new();

    public List<AuditLogModel> GetAll()
    {
        return _auditLogService
            .GetAll()
            .Where(IsReviewLog)
            .OrderByDescending(x => x.Timestamp)
            .ToList();
    }

    public List<AuditLogModel> GetByImage(
        string imageName)
    {
        if (string.IsNullOrWhiteSpace(imageName))
        {
            return new List<AuditLogModel>();
        }

        string value = imageName.Trim();

        return GetAll()
            .Where(x =>
                x.Description.Contains(
                    value,
                    StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public List<AuditLogModel> GetByAction(
        string action)
    {
        if (string.IsNullOrWhiteSpace(action))
        {
            return GetAll();
        }

        string value = action.Trim();

        return GetAll()
            .Where(x =>
                string.Equals(
                    x.Action,
                    value,
                    StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public List<AuditLogModel> GetRecent(
        int count = 100)
    {
        if (count <= 0)
        {
            count = 100;
        }

        return GetAll()
            .Take(count)
            .ToList();
    }

    private static bool IsReviewLog(
        AuditLogModel log)
    {
        if (log == null)
        {
            return false;
        }

        if (!string.Equals(
                log.Module,
                "RTR REVIEW",
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return string.Equals(
                   log.Action,
                   "ACCEPT_REVIEW",
                   StringComparison.OrdinalIgnoreCase)
               ||
               string.Equals(
                   log.Action,
                   "REJECT_REVIEW",
                   StringComparison.OrdinalIgnoreCase)
               ||
               string.Equals(
                   log.Action,
                   "HOLD_REVIEW",
                   StringComparison.OrdinalIgnoreCase)
               ||
               string.Equals(
                   log.Action,
                   "CREATE_REVIEW",
                   StringComparison.OrdinalIgnoreCase)
               ||
               string.Equals(
                   log.Action,
                   "DELETE_REVIEW",
                   StringComparison.OrdinalIgnoreCase);
    }
}