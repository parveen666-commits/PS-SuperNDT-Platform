using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class InspectionTransferService
{
    public ObservableCollection<InspectionTransferModel> Queue { get; } = new();

    public void AddToQueue(
        string jobNumber,
        string inspectionNumber,
        string imagePath,
        string destination,
        bool autoTransfer,
        string createdBy)
    {
        var fileInfo = File.Exists(imagePath)
            ? new FileInfo(imagePath)
            : null;

        Queue.Add(new InspectionTransferModel
        {
            Id = Guid.NewGuid(),
            JobNumber = jobNumber,
            InspectionNumber = inspectionNumber,
            ImagePath = imagePath,
            Destination = destination,
            CreatedOn = DateTime.Now,
            Status = TransferStatus.Pending,
            FileSizeBytes = fileInfo?.Length ?? 0,
            AutoTransfer = autoTransfer,
            CreatedBy = createdBy
        });
    }

    public void MarkQueued(Guid id)
    {
        var item = Queue.FirstOrDefault(x => x.Id == id);

        if (item is null)
            return;

        item.Status = TransferStatus.Queued;
    }

    public void MarkSending(Guid id)
    {
        var item = Queue.FirstOrDefault(x => x.Id == id);

        if (item is null)
            return;

        item.Status = TransferStatus.Sending;
        item.StartedOn = DateTime.Now;
    }

    public void MarkSent(Guid id)
    {
        var item = Queue.FirstOrDefault(x => x.Id == id);

        if (item is null)
            return;

        item.Status = TransferStatus.Sent;
        item.CompletedOn = DateTime.Now;
        item.ErrorMessage = string.Empty;
    }

    public void MarkFailed(Guid id, string error)
    {
        var item = Queue.FirstOrDefault(x => x.Id == id);

        if (item is null)
            return;

        item.Status = TransferStatus.Failed;
        item.CompletedOn = DateTime.Now;
        item.ErrorMessage = error;
    }

    public void Cancel(Guid id)
    {
        var item = Queue.FirstOrDefault(x => x.Id == id);

        if (item is null)
            return;

        item.Status = TransferStatus.Cancelled;
        item.CompletedOn = DateTime.Now;
    }
}