using System;
using System.Collections.ObjectModel;
using System.Linq;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class TransferSessionService
{
    public ObservableCollection<TransferSessionModel> Sessions { get; } = new();

    public TransferSessionModel StartSession(string sessionName)
    {
        var session = new TransferSessionModel
        {
            Id = Guid.NewGuid(),
            StartedOn = DateTime.Now,
            SessionName = sessionName,
            OperatorName = Environment.UserName,
            MachineName = Environment.MachineName,
            IsActive = true
        };

        Sessions.Insert(0, session);

        return session;
    }

    public void EndSession(
        Guid sessionId,
        int totalTransferred,
        int totalFailed,
        long totalBytesTransferred)
    {
        var session = Sessions.FirstOrDefault(x => x.Id == sessionId);

        if (session is null)
            return;

        session.EndedOn = DateTime.Now;
        session.TotalTransferred = totalTransferred;
        session.TotalFailed = totalFailed;
        session.TotalBytesTransferred = totalBytesTransferred;
        session.IsActive = false;
    }

    public TransferSessionModel? GetActiveSession()
    {
        return Sessions.FirstOrDefault(x => x.IsActive);
    }

    public void UpdateQueueCount(Guid sessionId, int queueCount)
    {
        var session = Sessions.FirstOrDefault(x => x.Id == sessionId);

        if (session is null)
            return;

        session.TotalQueued = queueCount;
    }

    public void Clear()
    {
        Sessions.Clear();
    }
}