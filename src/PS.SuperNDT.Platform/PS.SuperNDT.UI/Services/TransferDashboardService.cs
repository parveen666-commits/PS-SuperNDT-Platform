using System.Collections.ObjectModel;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class TransferDashboardService
{
    private readonly TransferEngineStatisticsService _statisticsService;

    public TransferDashboardService(
        TransferEngineStatisticsService statisticsService)
    {
        _statisticsService = statisticsService;
    }

    public ObservableCollection<TransferDashboardCardModel> BuildCards()
    {
        var stats = _statisticsService.GetStatistics();

        return new ObservableCollection<TransferDashboardCardModel>
        {
            new()
            {
                Title = "Pending",
                Value = stats.PendingCount.ToString(),
                Description = "Waiting For Transfer",
                Icon = "Clock",
                Status = "Pending"
            },

            new()
            {
                Title = "Sending",
                Value = stats.SendingCount.ToString(),
                Description = "Transfer In Progress",
                Icon = "Upload",
                Status = "Sending"
            },

            new()
            {
                Title = "Sent",
                Value = stats.SentCount.ToString(),
                Description = "Successfully Transferred",
                Icon = "Check",
                Status = "Success"
            },

            new()
            {
                Title = "Failed",
                Value = stats.FailedCount.ToString(),
                Description = "Transfer Failed",
                Icon = "Alert",
                Status = "Failed"
            },

            new()
            {
                Title = "Queue Size",
                Value = stats.TotalCount.ToString(),
                Description = "Total Queue Items",
                Icon = "List",
                Status = "Information"
            }
        };
    }
}