using System;
using System.Collections.ObjectModel;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class TransferAuditLogService
{
    public ObservableCollection<TransferAuditLogModel> Logs { get; } = new();

    public void Add(
        string eventType,
        string jobNumber,
        string inspectionNumber,
        string packageNumber,
        string source,
        string destination,
        string status,
        string message)
    {
        Logs.Insert(0, new TransferAuditLogModel
        {
            Id = Guid.NewGuid(),
            Timestamp = DateTime.Now,
            EventType = eventType,
            JobNumber = jobNumber,
            InspectionNumber = inspectionNumber,
            PackageNumber = packageNumber,
            UserName = Environment.UserName,
            MachineName = Environment.MachineName,
            Source = source,
            Destination = destination,
            Status = status,
            Message = message
        });
    }

    public void Clear()
    {
        Logs.Clear();
    }
}