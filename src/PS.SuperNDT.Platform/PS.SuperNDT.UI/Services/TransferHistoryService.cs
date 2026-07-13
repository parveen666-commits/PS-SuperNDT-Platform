using System;
using System.Collections.ObjectModel;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class TransferHistoryService
{
    public ObservableCollection<TransferHistoryModel> History { get; } = new();

    public void Add(
        InspectionTransferModel transfer,
        string remarks = "")
    {
        History.Insert(0, new TransferHistoryModel
        {
            Id = Guid.NewGuid(),
            JobNumber = transfer.JobNumber,
            InspectionNumber = transfer.InspectionNumber,
            FileName = System.IO.Path.GetFileName(transfer.ImagePath),
            SourcePath = transfer.ImagePath,
            DestinationPath = transfer.Destination,
            TransferDateTime = DateTime.Now,
            Status = transfer.Status,
            FileSizeBytes = transfer.FileSizeBytes,
            UserName = transfer.CreatedBy,
            Remarks = remarks
        });
    }

    public void Clear()
    {
        History.Clear();
    }
}