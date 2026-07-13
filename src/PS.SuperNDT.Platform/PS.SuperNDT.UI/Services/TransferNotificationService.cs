using System;
using System.Collections.ObjectModel;
using System.Linq;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class TransferNotificationService
{
    public ObservableCollection<TransferNotificationModel> Notifications { get; } = new();

    public void Add(
        string title,
        string message,
        string severity,
        string sourceModule,
        string jobNumber = "",
        string inspectionNumber = "")
    {
        Notifications.Insert(0, new TransferNotificationModel
        {
            Id = Guid.NewGuid(),
            Timestamp = DateTime.Now,
            Title = title,
            Message = message,
            Severity = severity,
            SourceModule = sourceModule,
            JobNumber = jobNumber,
            InspectionNumber = inspectionNumber,
            IsAcknowledged = false
        });
    }

    public void Acknowledge(Guid id)
    {
        var notification = Notifications.FirstOrDefault(x => x.Id == id);

        if (notification is null)
            return;

        notification.IsAcknowledged = true;
    }

    public void Remove(Guid id)
    {
        var notification = Notifications.FirstOrDefault(x => x.Id == id);

        if (notification is null)
            return;

        Notifications.Remove(notification);
    }

    public void Clear()
    {
        Notifications.Clear();
    }

    public int GetUnreadCount()
    {
        return Notifications.Count(x => !x.IsAcknowledged);
    }
}