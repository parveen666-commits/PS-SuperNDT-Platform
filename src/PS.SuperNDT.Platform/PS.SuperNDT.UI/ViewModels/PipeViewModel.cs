using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PS.SuperNDT.UI.Models;
using PS.SuperNDT.UI.Services;

namespace PS.SuperNDT.UI.ViewModels;

public sealed class PipeViewModel : INotifyPropertyChanged
{
    private readonly PipeService _pipeService = new();

    private PipeModel? _selectedPipe;
    private Guid _currentWorkOrderId;
    private string _searchText = string.Empty;

    public ObservableCollection<PipeModel> Pipes { get; } = new();

    public PipeModel? SelectedPipe
    {
        get => _selectedPipe;
        set
        {
            _selectedPipe = value;
            OnPropertyChanged();
        }
    }

    public Guid CurrentWorkOrderId
    {
        get => _currentWorkOrderId;
        private set
        {
            if (_currentWorkOrderId == value)
                return;

            _currentWorkOrderId = value;
            OnPropertyChanged();
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText == value)
                return;

            _searchText = value ?? string.Empty;
            OnPropertyChanged();

            Refresh();
        }
    }

    public int TotalPipes =>
        Pipes.Count;

    public int PendingPipes =>
        CountStatus("PENDING");

    public int RtInProgressPipes =>
        CountStatus("RT_IN_PROGRESS");

    public int ReviewPendingPipes =>
        CountStatus("REVIEW_PENDING");

    public int RepairPipes =>
        CountStatus("REPAIR");

    public int AcceptedPipes =>
        Pipes.Count(x => x.IsAccepted);

    public int RejectedPipes =>
        Pipes.Count(x => x.IsRejected);

    // ============================================================
    // LOAD
    // ============================================================

    public void Load(Guid workOrderId)
    {
        CurrentWorkOrderId = workOrderId;

        Refresh();
    }

    // ============================================================
    // REFRESH
    // ============================================================

    public void Refresh()
    {
        if (CurrentWorkOrderId == Guid.Empty)
            return;

        Pipes.Clear();

        var pipes = string.IsNullOrWhiteSpace(SearchText)
            ? _pipeService.GetByWorkOrder(CurrentWorkOrderId)
            : _pipeService.Search(
                CurrentWorkOrderId,
                SearchText);

        foreach (var pipe in pipes)
        {
            Pipes.Add(pipe);
        }

        RaiseSummaryProperties();
    }

    // ============================================================
    // ADD NEW PIPE
    // ============================================================

    public PipeModel AddNew()
    {
        var pipe = new PipeModel
        {
            WorkOrderId = CurrentWorkOrderId,
            PipeNumber = GetNextPipeNumber(),
            Status = "PENDING",
            Result = "PENDING",
            CreatedOn = DateTime.Now
        };

        _pipeService.Save(pipe);

        Refresh();

        SelectedPipe = pipe;

        return pipe;
    }

    // ============================================================
    // SAVE PIPE
    // ============================================================

    public bool SavePipe(PipeModel pipe)
    {
        ArgumentNullException.ThrowIfNull(pipe);

        if (CurrentWorkOrderId == Guid.Empty)
            return false;

        pipe.WorkOrderId = CurrentWorkOrderId;

        if (string.IsNullOrWhiteSpace(pipe.PipeNumber))
        {
            pipe.PipeNumber = GetNextPipeNumber();
        }

        if (_pipeService.ExistsPipeNumber(
                pipe.WorkOrderId,
                pipe.PipeNumber,
                pipe.Id))
        {
            return false;
        }

        if (pipe.Id == Guid.Empty)
        {
            pipe.Id = Guid.NewGuid();
            pipe.CreatedOn = DateTime.Now;
        }

        pipe.UpdatedOn = DateTime.Now;

        _pipeService.Save(pipe);

        Refresh();

        SelectedPipe =
            FindPipe(pipe.Id);

        return true;
    }

    // ============================================================
    // DELETE
    // ============================================================

    public bool DeleteSelected()
    {
        if (SelectedPipe == null)
            return false;

        var id = SelectedPipe.Id;

        _pipeService.Delete(id);

        SelectedPipe = null;

        Refresh();

        return true;
    }

    // ============================================================
    // SEARCH
    // ============================================================

    public void ClearSearch()
    {
        if (string.IsNullOrEmpty(SearchText))
            return;

        SearchText = string.Empty;
    }

    // ============================================================
    // RT WORKFLOW
    // ============================================================

    public bool StartRT(
        string operatorName = "")
    {
        if (SelectedPipe == null)
            return false;

        _pipeService.StartRT(
            SelectedPipe.Id,
            operatorName);

        RefreshSelection();

        return true;
    }

    public bool CompleteRT()
    {
        if (SelectedPipe == null)
            return false;

        _pipeService.CompleteRT(
            SelectedPipe.Id);

        RefreshSelection();

        return true;
    }

    // ============================================================
    // REVIEW WORKFLOW
    // ============================================================

    public bool StartReview(
        string inspector = "")
    {
        if (SelectedPipe == null)
            return false;

        _pipeService.StartReview(
            SelectedPipe.Id,
            inspector);

        RefreshSelection();

        return true;
    }

    public bool MarkPending()
    {
        if (SelectedPipe == null)
            return false;

        _pipeService.MarkPending(
            SelectedPipe.Id);

        RefreshSelection();

        return true;
    }

    public bool MarkAccepted(
        string inspector = "")
    {
        if (SelectedPipe == null)
            return false;

        _pipeService.MarkAccepted(
            SelectedPipe.Id,
            inspector);

        RefreshSelection();

        return true;
    }

    public bool MarkRejected(
        string inspector = "")
    {
        if (SelectedPipe == null)
            return false;

        _pipeService.MarkRejected(
            SelectedPipe.Id,
            inspector);

        RefreshSelection();

        return true;
    }

    // ============================================================
    // REPAIR
    // ============================================================

    public bool MarkRepair(
        string repairLocation,
        string repairRemark = "")
    {
        if (SelectedPipe == null)
            return false;

        _pipeService.MarkRepair(
            SelectedPipe.Id,
            repairLocation,
            repairRemark);

        RefreshSelection();

        return true;
    }

    public bool CompleteRepair()
    {
        if (SelectedPipe == null)
            return false;

        _pipeService.CompleteRepair(
            SelectedPipe.Id);

        RefreshSelection();

        return true;
    }

    // ============================================================
    // SHOT SUMMARY
    // ============================================================

    public bool UpdateShotSummary(
        int totalShots,
        int completedShots,
        int pendingShots,
        int defectShots)
    {
        if (SelectedPipe == null)
            return false;

        _pipeService.UpdateShotSummary(
            SelectedPipe.Id,
            totalShots,
            completedShots,
            pendingShots,
            defectShots);

        RefreshSelection();

        return true;
    }

    // ============================================================
    // WELD SUMMARY
    // ============================================================

    public bool UpdateWeldSummary(
        int totalWelds,
        int completedWelds)
    {
        if (SelectedPipe == null)
            return false;

        _pipeService.UpdateWeldSummary(
            SelectedPipe.Id,
            totalWelds,
            completedWelds);

        RefreshSelection();

        return true;
    }

    // ============================================================
    // STORAGE
    // ============================================================

    public bool UpdateStoragePaths(
        string storagePath,
        string reviewPath,
        string diconPath)
    {
        if (SelectedPipe == null)
            return false;

        _pipeService.UpdateStoragePaths(
            SelectedPipe.Id,
            storagePath,
            reviewPath,
            diconPath);

        RefreshSelection();

        return true;
    }

    // ============================================================
    // CLOSE PIPE
    // ============================================================

    public bool CloseSelected()
    {
        if (SelectedPipe == null)
            return false;

        _pipeService.Close(
            SelectedPipe.Id);

        RefreshSelection();

        return true;
    }

    // ============================================================
    // HELPERS
    // ============================================================

    private PipeModel? FindPipe(Guid id)
    {
        foreach (var pipe in Pipes)
        {
            if (pipe.Id == id)
                return pipe;
        }

        return null;
    }

    private void RefreshSelection()
    {
        var selectedId =
            SelectedPipe?.Id ?? Guid.Empty;

        Refresh();

        if (selectedId != Guid.Empty)
        {
            SelectedPipe =
                FindPipe(selectedId);
        }
    }

    private string GetNextPipeNumber()
    {
        var pipes =
            _pipeService.GetByWorkOrder(
                CurrentWorkOrderId);

        var maxNumber = 0;

        foreach (var pipe in pipes)
        {
            if (!pipe.PipeNumber.StartsWith(
                    "P-",
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var numberText =
                pipe.PipeNumber[2..];

            if (int.TryParse(
                    numberText,
                    out var number))
            {
                if (number > maxNumber)
                    maxNumber = number;
            }
        }

        return $"P-{maxNumber + 1:000}";
    }

    private int CountStatus(string status)
    {
        var count = 0;

        foreach (var pipe in Pipes)
        {
            if (string.Equals(
                    pipe.Status,
                    status,
                    StringComparison.OrdinalIgnoreCase))
            {
                count++;
            }
        }

        return count;
    }

    private void RaiseSummaryProperties()
    {
        OnPropertyChanged(nameof(TotalPipes));
        OnPropertyChanged(nameof(PendingPipes));
        OnPropertyChanged(nameof(RtInProgressPipes));
        OnPropertyChanged(nameof(ReviewPendingPipes));
        OnPropertyChanged(nameof(RepairPipes));
        OnPropertyChanged(nameof(AcceptedPipes));
        OnPropertyChanged(nameof(RejectedPipes));
    }

    // ============================================================
    // PROPERTY CHANGED
    // ============================================================

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(
                propertyName));
    }
}