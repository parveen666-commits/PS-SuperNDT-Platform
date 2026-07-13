using System;
using System.Collections.ObjectModel;
using PS.SuperNDT.UI.Models;

namespace PS.SuperNDT.UI.Services;

public sealed class ReportChecklistService
{
    private readonly ObservableCollection<ReportChecklistItemModel> _items;

    public ReportChecklistService()
    {
        _items = new ObservableCollection<ReportChecklistItemModel>();
    }

    public ReadOnlyObservableCollection<ReportChecklistItemModel> Items =>
        new(_items);

    public void CreateDefaultChecklist(
        Guid reportId)
    {
        _items.Clear();

        _items.Add(new ReportChecklistItemModel
        {
            ReportId = reportId,
            ChecklistName = "Equipment Calibration",
            Description = "Verify detector, source and measuring equipment calibration."
        });

        _items.Add(new ReportChecklistItemModel
        {
            ReportId = reportId,
            ChecklistName = "Procedure Verification",
            Description = "Confirm approved inspection procedure is available."
        });

        _items.Add(new ReportChecklistItemModel
        {
            ReportId = reportId,
            ChecklistName = "Image Quality",
            Description = "Verify IQI visibility and image quality requirements."
        });

        _items.Add(new ReportChecklistItemModel
        {
            ReportId = reportId,
            ChecklistName = "Final Evaluation",
            Description = "Complete acceptance/rejection evaluation."
        });
    }

    public void CompleteItem(
        Guid itemId,
        string checkedBy,
        string remarks)
    {
        var item = Find(itemId);

        if (item == null)
        {
            return;
        }

        item.IsCompleted = true;
        item.CheckedBy = checkedBy;
        item.CheckedOn = DateTime.Now;
        item.Remarks = remarks;
    }

    private ReportChecklistItemModel? Find(
        Guid itemId)
    {
        foreach (var item in _items)
        {
            if (item.Id == itemId)
            {
                return item;
            }
        }

        return null;
    }
}