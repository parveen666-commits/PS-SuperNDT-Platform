using System;
using System.Linq;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportStatisticsService
{
    private readonly ReportStorageService _storageService;

    public ReportStatisticsService()
    {
        _storageService = new ReportStorageService();
    }

    public int GetTotalReports()
    {
        return _storageService.GetAll().Count;
    }

    public int GetApprovedReports()
    {
        return _storageService
            .GetAll()
            .Count(x => x.IsApproved);
    }

    public int GetPendingReports()
    {
        return _storageService
            .GetAll()
            .Count(x => !x.IsApproved);
    }

    public int GetTodayReports()
    {
        var today = DateTime.Today;

        return _storageService
            .GetAll()
            .Count(x => x.GeneratedDate.Date == today);
    }

    public int GetTotalFindings()
    {
        return _storageService
            .GetAll()
            .Sum(x => x.Findings.Count);
    }

    public int GetTotalImages()
    {
        return _storageService
            .GetAll()
            .Sum(x => x.Images.Count);
    }
}