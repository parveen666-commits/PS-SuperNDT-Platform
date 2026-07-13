using System;
using System.Collections.ObjectModel;
using System.Linq;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class WorklistService
{
    public ObservableCollection<WorklistItemModel> Worklist { get; } = new();

    public WorklistItemModel Create(
        string jobNumber,
        string inspectionNumber)
    {
        var item = new WorklistItemModel
        {
            Id = Guid.NewGuid(),
            JobNumber = jobNumber,
            InspectionNumber = inspectionNumber,
            CreatedOn = DateTime.Now,
            TransferStatus = TransferStatus.Pending
        };

        Worklist.Add(item);

        return item;
    }

    public WorklistItemModel? Get(Guid id)
    {
        return Worklist.FirstOrDefault(x => x.Id == id);
    }

    public void MarkReady(Guid id)
    {
        var item = Get(id);

        if (item is null)
            return;

        item.ReadyForTransfer = true;
    }

    public void UpdateTransferStatus(
        Guid id,
        TransferStatus status)
    {
        var item = Get(id);

        if (item is null)
            return;

        item.TransferStatus = status;
    }

    public void Remove(Guid id)
    {
        var item = Get(id);

        if (item is null)
            return;

        Worklist.Remove(item);
    }

    public void Clear()
    {
        Worklist.Clear();
    }
}